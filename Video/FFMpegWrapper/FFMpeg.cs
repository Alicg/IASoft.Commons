using System;
using System.Collections.Generic;
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
        private readonly PresetParameters presetParameters;
        private readonly IObservable<double> stopSignal;
        private static readonly string pathToFfMpegExe;
        private static readonly string fontsPath;
        public static bool DebugModeEnabled = false;
        private const string DefaultFlags = "+ildct+ilme";

        static FFMpeg()
        {
            const string AppDir = "";
            pathToFfMpegExe = Path.Combine(AppDir, "ffmpeg.exe");
            if (!File.Exists(pathToFfMpegExe))
            {
                typeof(FFMpeg).Assembly.GetManifestResourceStream("FFMpegWrapper.ffmpeg.exe").WriteToFile(pathToFfMpegExe);
            }

            fontsPath = Path.Combine(AppDir, "arialbd.ttf");
            if (!File.Exists(fontsPath))
            {
                typeof(FFMpeg).Assembly.GetManifestResourceStream("FFMpegWrapper.arialbd.ttf").WriteToFile(fontsPath);
            }
        }

        public FFMpeg(PresetParameters presetParameters = PresetParameters.Medium, IObservable<double> stopSignal = null)
        {
            this.presetParameters = presetParameters;
            this.stopSignal = stopSignal ?? Observable.Empty<double>();
        }

        public void Concat(string outputFile, IGlobalExportProgress globalExportProgress, params string[] inputFiles)
        {
            EnsureFileDoesNotExist(outputFile);
            var result = new FFMpegCommandBuilder()
                .ConcatInputsFrom(inputFiles)
                .OutputVideoCodec("libx264")
                .OutputPreset(this.presetParameters)
                .OutputTune("fastdecode")
                .WithFlags(DefaultFlags)
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .BuildCommand(pathToFfMpegExe)
                .Execute();
            this.LogMessage("CONCAT", result);
            globalExportProgress.IncreaseOperationsDone();
        }

        public void Cut(double start, double duration, string inputFile, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            var result = new FFMpegCommandBuilder()
                .InputFrom(inputFile)
                .StartFrom(start)
                .DurationIs(duration)
                .OutputVideoCodec("libx264")
                //.OutputAudioCodec("libmp3lame")
                //.OutputSize(1024, 768)
                //.OutputFrameRate(60)
                .OutputPreset(this.presetParameters)
                .OutputTune("fastdecode")
                .WithFlags(DefaultFlags)
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .BuildCommand(pathToFfMpegExe)
                .Execute();
            this.LogMessage("CUT", result);
            globalExportProgress.IncreaseOperationsDone();
        }

        public void DrawImage(string inputFile, List<DrawImageTimeRecord> imagesTimeTable, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            if (!imagesTimeTable.Any())
                return;
            EnsureFileDoesNotExist(outputFile);
            var result = new FFMpegCommandBuilder()
                .InputFrom(inputFile)
                .DrawImages(imagesTimeTable)
                .OutputVideoCodec("libx264")
                .WithFlags(DefaultFlags)
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .BuildCommand(pathToFfMpegExe)
                .Execute();
            this.LogMessage("DrawImage", result);
            globalExportProgress.IncreaseOperationsDone();
        }

        public void DrawText(string inputFile, string overlayText, string outputFile, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            const int FontSize = 30;
            const int CharsInLine = 40;
            var lines = GetTranspositionedText(overlayText, CharsInLine);
            var result = new FFMpegCommandBuilder()
                .InputFrom(inputFile)
                .DrawText(lines, fontsPath, FontSize)
                .OutputVideoCodec("libx264")
                .OutputTune("fastdecode")
                .OutputPreset(this.presetParameters)
                .WithFlags(DefaultFlags)
                .OutputTo(outputFile)
                .WithProgressCallback(globalExportProgress.SetCurrentOperationProgress)
                .WithStopSignal(this.stopSignal)
                .BuildCommand(pathToFfMpegExe)
                .Execute();
            this.LogMessage("DrawText", result);
            globalExportProgress.IncreaseOperationsDone();
        }

        public void CutAndDrawText(string inputFile, int start, int end, string outputFile, string overlayText, IGlobalExportProgress globalExportProgress)
        {
            EnsureFileDoesNotExist(outputFile);
            if (string.IsNullOrEmpty(overlayText))
            {
                this.Cut(start, end, inputFile, outputFile, globalExportProgress);
                return;
            }
            var lastDot = inputFile.LastIndexOf('.');
            var inputExt = inputFile.Substring(lastDot);
            var intermediateFile = GetIntermediateFile(inputExt);
            this.Cut(start, end, inputFile, intermediateFile, globalExportProgress);
            this.DrawText(intermediateFile, overlayText, outputFile, globalExportProgress);
            File.Delete(intermediateFile);
        }

        public byte[] GetBitmapFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize)
        {
            var intermediateFile = GetIntermediateFile(".jpg");
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
                var result = command.Execute();
                this.LogMessage("ExtractFrame", result);
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
            finally
            {
                File.Delete(intermediateFile);
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
            var result = command.Execute();
            this.LogMessage("GetVideoInfo", result);
            return ParseVideoInfo(result);
        }

        public void LogMessage(string title, string message)
        {
            if (DebugModeEnabled)
            {
                FFMpegLogger.Instance.Info($"{DateTime.Now}\r\n-----------{title}:--------------\r\n{message}\r\n---------------------------\r\n");
            }
        }

        private static void EnsureFileDoesNotExist(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static string GetIntermediateFile(string ext)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid()}{ext}");
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
            var width = Convert.ToInt32(match.Groups["Width"].Value);
            var height = Convert.ToInt32(match.Groups["Height"].Value);
            var videoInfo = new FFMpegVideoInfo(width, height);
            return videoInfo;
        }
    }
}