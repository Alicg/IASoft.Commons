using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using FFMpegExecutable;
using Utils.Extensions;

namespace FFMpegWrapper
{
    public class FFMpeg : IDisposable
    {
        private readonly TemporaryFilesStorage temporaryFilesStorage;
        private readonly ProcessPriorityClass ffmpegProcessPriorityClass;

        private readonly IObservable<double> stopSignal;
        private static readonly string pathToFfMpegExe;
        private static readonly string fontsPath;
        public static bool DebugModeEnabled = true;
        
        static FFMpeg()
        {
            const string AppDir = "";
            pathToFfMpegExe = FFMpegExeLoader.UnpackFFMpegExe();

            fontsPath = Path.Combine(AppDir, "arialbd.ttf");
            if (!File.Exists(fontsPath))
            {
                typeof(FFMpeg).Assembly.GetManifestResourceStream("FFMpegWrapper.arialbd.ttf").WriteToFile(fontsPath);
            }
        }

        public FFMpeg(TemporaryFilesStorage temporaryFilesStorage,
            ProcessPriorityClass ffmpegProcessPriorityClass = ProcessPriorityClass.High,
            IObservable<double> stopSignal = null)
        {
            this.temporaryFilesStorage = temporaryFilesStorage;
            this.ffmpegProcessPriorityClass = ffmpegProcessPriorityClass;
            this.stopSignal = stopSignal ?? Observable.Empty<double>();
        }

        public void Convert(string inputFile, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .AppendCustom("-fflags +genpts")
                .InputFrom(inputFile)
                .OutputVideoCodec("copy")
                .OutputAudioCodec("copy -map 0")
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "CONVERT");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void Concat(string outputFile, string vCodec, string aCodec, IGlobalExportProgress globalExportProgress, params string[] inputFiles)
        {
            this.Concat(outputFile, Size.Empty, vCodec, aCodec, globalExportProgress, inputFiles);
        }

        public void Concat(string outputFile, Size outputSize, string vCodec, string aCodec, IGlobalExportProgress globalExportProgress, params string[] inputFiles)
        {
            EnsureFileDoesNotExist(outputFile);
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .ConcatInputsFrom(inputFiles)
                .OutputVideoCodec(vCodec)
                .OutputPreset(PresetParameters.SuperFast)
                .OutputAudioCodec(aCodec)
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "CONCAT");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void Cut(FFMpegCutOptions cutOptions, bool muteProgress = false)
        {
            EnsureFileDoesNotExist(cutOptions.OutputFile);
            var cutCommandBuilder = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .StartFrom(cutOptions.Start)
                .InputFrom(cutOptions.InputFile)
                .DurationIs(cutOptions.Duration)
                .OutputVideoCodec(cutOptions.VideoCodec)
                .OutputAudioCodec(cutOptions.AudioCodec);
            if (!cutOptions.SimpleMode)
            {
                cutCommandBuilder = cutCommandBuilder.OutputScale(cutOptions.OutputSize);
            }
            cutCommandBuilder = cutCommandBuilder
                .AppendCustom("-avoid_negative_ts 1 -max_muxing_queue_size 1000")
                .OutputTo(cutOptions.OutputFile);
            if (!muteProgress)
            {
                cutCommandBuilder = cutCommandBuilder.WithProgressCallback(cutOptions.GlobalExportProgress.SetCurrentOperationProgress);
            }
            var command = cutCommandBuilder.WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "CUT");
            if (!muteProgress)
            {
                cutOptions.GlobalExportProgress.IncreaseOperationsDone(command.ProcessId);
            }
        }

        public void DrawImage(string inputFile, List<DrawImageTimeRecord> imagesTimeTable, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            if (!imagesTimeTable.Any())
                return;
            EnsureFileDoesNotExist(outputFile);
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .InputFrom(inputFile)
                .DrawImages(imagesTimeTable)
                .OutputVideoCodec(FFMpegCutOptions.DefaultVideoCodec)
                .OutputAudioCodec(FFMpegCutOptions.DefaultAudioCodec)    
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "DrawImage");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void DrawText(string inputFile, List<TextTimeRecord> overlayText, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            const int FontSize = 30;
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .InputFrom(inputFile)
                .DrawText(overlayText, fontsPath, FontSize)
                .OutputVideoCodec(FFMpegCutOptions.DefaultVideoCodec)
                .OutputAudioCodec(FFMpegCutOptions.DefaultAudioCodec) 
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "DrawText");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void ApplyTimeWarp(string inputFile, IList<TimeWarpRecord> timeWarps, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            var cutCommandBuilder = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .InputFrom(inputFile)
                .ApplyTimeWarp(timeWarps)
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass);
            var command = cutCommandBuilder.BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "TIME_WARP");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId, 1);
        }

        public void CutAndConcatAndDrawImagesAndText(IList<FFMpegCutInfo> cutInfosToConcat, IList<DrawImageTimeRecord> imagesTimeTable, List<TextTimeRecord> overlayText, Size finalScale, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            const int FontSizeFor1024Width = 30;
            var fontSize = finalScale.IsEmpty ? FontSizeFor1024Width : ((double)finalScale.Width / 1024) * FontSizeFor1024Width;
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .CutConcatDrawImagesAndText(cutInfosToConcat, imagesTimeTable, overlayText, finalScale, fontsPath, (int)fontSize)
                .OutputVideoCodec(FFMpegCutOptions.DefaultVideoCodec)
                .OutputPreset(PresetParameters.SuperFast)
                .OutputAudioCodec(FFMpegCutOptions.DefaultAudioCodec)
                .Fix2FramesLeftError()
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe, cutInfosToConcat.Aggregate(0.0, (t,c) => t + c.EndSecond - c.StartSecond));

            this.ExecuteFFMpegCommand(command, "DrawText");
            
            globalExportProgress.IncreaseOperationsDone(command.ProcessId, 1);
        }

        public void ConcatAndDrawImagesAndText(IList<string> inputFilesToConcat, IList<DrawImageTimeRecord> imagesTimeTable, List<TextTimeRecord> overlayText, Size finalScale, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            const int FontSizeFor1024Width = 30;
            var fontSize = finalScale.IsEmpty ? FontSizeFor1024Width : ((double)finalScale.Width / 1024) * FontSizeFor1024Width;
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .ConcatDrawImagesAndText(inputFilesToConcat, imagesTimeTable, overlayText, finalScale, fontsPath, (int)fontSize)
                .OutputVideoCodec(FFMpegCutOptions.DefaultVideoCodec)
                .OutputPreset(PresetParameters.SuperFast)
                .OutputAudioCodec(FFMpegCutOptions.DefaultAudioCodec) 
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "DrawText");
            
            //одна операция для concat-a без постэффектов.
            var operationsCountForConcat = 1;
            var operationsCountForConcatAndDrawImagesAndText =
                Math.Max(operationsCountForConcat, Math.Max(imagesTimeTable.Count, overlayText.Count)) * 2;
            globalExportProgress.IncreaseOperationsDone(command.ProcessId, operationsCountForConcatAndDrawImagesAndText);
        }

        public void DrawImagesAndText(string inputFile, IList<DrawImageTimeRecord> imagesTimeTable, List<TextTimeRecord> overlayText, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            const int FontSize = 30;
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .InputFrom(inputFile)
                .DrawImagesAndText(imagesTimeTable, overlayText, fontsPath, FontSize)
                .OutputVideoCodec(FFMpegCutOptions.DefaultVideoCodec)
                .OutputPreset(PresetParameters.SuperFast)
                .OutputAudioCodec(FFMpegCutOptions.DefaultAudioCodec) 
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .WithPriority(this.ffmpegProcessPriorityClass)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "DrawText");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId, Math.Max(imagesTimeTable.Count, overlayText.Count));
        }

        public void CutAndDrawText(FFMpegCutOptions cutOptions, List<TextTimeRecord> overlayText)
        {
            EnsureFileDoesNotExist(cutOptions.OutputFile);
            if (!overlayText.Any())
            {
                this.Cut(cutOptions);
                return;
            }
            var lastDot = cutOptions.InputFile.LastIndexOf('.');
            var inputExt = cutOptions.InputFile.Substring(lastDot);
            var intermediateFile = this.temporaryFilesStorage.GetIntermediateFile(inputExt);
            this.Cut(cutOptions.CloneWithOtherOutput(intermediateFile));
            this.DrawText(intermediateFile, overlayText, cutOptions.OutputFile, cutOptions.GlobalExportProgress);
        }

        public byte[] GetBitmapFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize)
        {
            var intermediateFile = this.temporaryFilesStorage.GetIntermediateFile(".jpg");
            try
            {
                var quality = imageSize.ToString().ToLower();
                var parameters = string.Format(
                    CultureInfo.InvariantCulture,
                    "-ss {1} -i \"{0}\" -vframes 1 -s {2} \"{3}\"",
                    videoFile,
                    position,
                    quality,
                    intermediateFile);
                var command = new FFMpegCommand(pathToFfMpegExe, parameters, true);
                this.ExecuteFFMpegCommand(command, "ExtractFrame");
                
                using (var fs = new FileStream(intermediateFile, FileMode.Open, FileAccess.Read))
                {
                    var byteImage = new byte[fs.Length];
                    fs.Read(byteImage, 0, (int)fs.Length);
                    this.LogMessage("ExtractFrame", "RETURN: " + byteImage.Length);

                    return byteImage;
                }
            }
            catch (Exception e)
            {
                this.LogMessage("ExtractFrame error:", e.GetFullMessage());
                throw;
            }
        }

        public FFMpegVideoInfo GetVideoInfo(string inputFile)
        {
            if (string.IsNullOrEmpty(inputFile) || (!inputFile.Contains("http://") && !inputFile.Contains("https://") && !File.Exists(inputFile)))
            {
                throw new FileNotFoundException($"Can't get video info. File path is null or file doesn't exist. Path: {inputFile}");
            }

            // информация о видео придёт вместе с сообщением об ошибке, что не задан выходной файл.
            var command = new FFMpegCommand(pathToFfMpegExe, $"-i \"{inputFile}\"", true);
            var result = this.ExecuteFFMpegCommand(command, "GetVideoInfo");
            return ParseVideoInfo(result);
        }

        public void LogMessage(string title, string message)
        {
            if (DebugModeEnabled)
            {
                FFMpegLogger.Instance.Info($"{DateTime.Now}\r\n-----------{title}:--------------\r\n{message}\r\n---------------------------\r\n");
            }
        }

        private string ExecuteFFMpegCommand(FFMpegCommand command, string operationName)
        {
            try
            {
                var result = command.Execute();
                this.LogMessage(operationName, result);
                return result;
            }
            catch (FFMpegException ffMpegException)
            {
                this.LogMessage(operationName, ffMpegException.AllFFMpegOutput);
                throw;
            }
            catch (Exception exception)
            {
                this.LogMessage(operationName + " UNKNOWN ERROR", exception.Message);
                throw;
            }
        }

        private static void EnsureFileDoesNotExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static FFMpegVideoInfo ParseVideoInfo(string str)
        {
            return new VideoInfoParser().ParseFFMpegInfo(str);
        }

        public void Dispose()
        {
            this.temporaryFilesStorage?.Dispose();
        }
    }
}