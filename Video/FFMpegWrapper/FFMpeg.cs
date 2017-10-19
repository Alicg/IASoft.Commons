using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Utils.Extensions;

namespace FFMpegWrapper
{
    public class FFMpeg
    {
        private readonly TemporaryFilesStorage temporaryFilesStorage;

        private readonly IObservable<double> stopSignal;
        private static readonly string pathToFfMpegExe;
        private static readonly string fontsPath;
        public static bool DebugModeEnabled = true;
        
        static FFMpeg()
        {
            const string AppDir = "";
            pathToFfMpegExe = Path.Combine(AppDir, "ffmpeg.exe");
            var ffMpegFromResources = typeof(FFMpeg).Assembly.GetManifestResourceStream("FFMpegWrapper.ffmpeg.exe");
            if (ffMpegFromResources == null)
            {
                throw new FFMpegException("FFMpegWrapper.ffmpeg.exe wasn't found in resources.", string.Empty);
            }
            if (!File.Exists(pathToFfMpegExe))
            {
                ffMpegFromResources.WriteToFile(pathToFfMpegExe);
            }
            else
            {
                var existedFileSize = new FileInfo(pathToFfMpegExe).Length;
                if (ffMpegFromResources.Length != existedFileSize)
                {
                    ffMpegFromResources.WriteToFile(pathToFfMpegExe);
                }
            }

            fontsPath = Path.Combine(AppDir, "arialbd.ttf");
            if (!File.Exists(fontsPath))
            {
                typeof(FFMpeg).Assembly.GetManifestResourceStream("FFMpegWrapper.arialbd.ttf").WriteToFile(fontsPath);
            }
        }

        public FFMpeg(TemporaryFilesStorage temporaryFilesStorage, IObservable<double> stopSignal = null)
        {
            this.temporaryFilesStorage = temporaryFilesStorage;
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
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "CONVERT");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void Concat(string outputFile, IGlobalExportProgress globalExportProgress, params string[] inputFiles)
        {
            this.Concat(outputFile, Size.Empty, globalExportProgress, inputFiles);
        }

        public void Concat(string outputFile, Size outputSize, IGlobalExportProgress globalExportProgress, params string[] inputFiles)
        {
            EnsureFileDoesNotExist(outputFile);
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .ConcatInputsFrom(inputFiles)
                .OutputVideoCodec("copy")
                .OutputAudioCodec("copy")
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "CONCAT");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void Cut(FFMpegCutOptions cutOptions)
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
            var command = cutCommandBuilder
                .AppendCustom("-avoid_negative_ts 1 -max_muxing_queue_size 1000")
                .OutputTo(cutOptions.OutputFile)
                .WithProgressCallback(cutOptions.GlobalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "CUT");
            cutOptions.GlobalExportProgress.IncreaseOperationsDone(command.ProcessId);
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
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "DrawImage");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void DrawText(string inputFile, string overlayText, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            const int FontSize = 30;
            const int CharsInLine = 40;
            var lines = GetTranspositionedText(overlayText, CharsInLine);
            var command = new FFMpegCommandBuilder(this.temporaryFilesStorage)
                .InputFrom(inputFile)
                .DrawText(lines, fontsPath, FontSize)
                .OutputVideoCodec(FFMpegCutOptions.DefaultVideoCodec)
                .OutputAudioCodec(FFMpegCutOptions.DefaultAudioCodec) 
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .BuildCommand(pathToFfMpegExe);

            this.ExecuteFFMpegCommand(command, "DrawText");
            globalExportProgress.IncreaseOperationsDone(command.ProcessId);
        }

        public void CutAndDrawText(FFMpegCutOptions cutOptions, string overlayText)
        {
            EnsureFileDoesNotExist(cutOptions.OutputFile);
            if (string.IsNullOrEmpty(overlayText))
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
            if (string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
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

        private static string[] GetTranspositionedText(string text, int charsInLine)
        {
            if (text.Length <= charsInLine)
                return new[] {text};
            var lastSpacePosition = 0;
            while (true)
            {
                var spacePosition = text.IndexOf(' ', lastSpacePosition + 1, charsInLine - lastSpacePosition);
                if(spacePosition == -1)
                    break;
                lastSpacePosition = spacePosition;
            }
            var oneLineText = text.Substring(0, lastSpacePosition);
            var remainingLines = GetTranspositionedText(text.Substring(lastSpacePosition + 1, text.Length - lastSpacePosition - 1), charsInLine);
            return new[] {oneLineText}.Union(remainingLines).ToArray();
        }

        private static FFMpegVideoInfo ParseVideoInfo(string str)
        {
            var regex = new Regex(@"Video:[^~]*?(?<Width>\d{3,5})x(?<Height>\d{3,5})");
            var match = regex.Match(str);
            var width = System.Convert.ToInt32(match.Groups["Width"].Value);
            var height = System.Convert.ToInt32(match.Groups["Height"].Value);
            var videoInfo = new FFMpegVideoInfo(width, height);
            return videoInfo;
        }
    }
}