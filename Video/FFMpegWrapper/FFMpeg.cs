using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Utils.Extensions;

namespace FFMpegWrapper
{
    public class FFMpeg
    {
        private readonly PresetParameters presetParameters;
        private readonly string pathToFfMpegExe;
        private readonly string fontsPath;
        public static string LogFile = "ffmpegwrapper.log";
        public static bool DebugModeEnabled = false;

        public FFMpeg(PresetParameters presetParameters = PresetParameters.Medium)
        {
            this.presetParameters = presetParameters;
            const string AppDir = "";
            this.pathToFfMpegExe = Path.Combine(AppDir, "ffmpeg.exe");
            if (!File.Exists(this.pathToFfMpegExe))
            {
                typeof(FFMpeg).Assembly.GetManifestResourceStream("FFMpegWrapper.ffmpeg.exe").WriteToFile(this.pathToFfMpegExe);
            }

            this.fontsPath = Path.Combine(AppDir, "arialbd.ttf");
            if (!File.Exists(this.fontsPath))
            {
                typeof(FFMpeg).Assembly.GetManifestResourceStream("FFMpegWrapper.arialbd.ttf").WriteToFile(this.fontsPath);
            }
        }

        public FFMpeg(string pathToFFMpegExe, string fontsPath, PresetParameters presetParameters = PresetParameters.Medium)
        {
            this.presetParameters = presetParameters;
            this.pathToFfMpegExe = pathToFFMpegExe;
            this.fontsPath = fontsPath;
        }

        public void Concat(string outputFile, params string[] inputFiles)
        {
            EnsureFileDoesNotExist(outputFile);
            var result = new FFMpegCommandBuilder()
                .ConcatInputsFrom(inputFiles)
                .OutputVideoCodec("libx264")
                .OutputPreset(this.presetParameters)
                .OutputTune("fastdecode")
                .WithFlags("+ildct+ilme")
                .OutputTo(outputFile)
                .BuildCommand(this.pathToFfMpegExe)
                .Execute();
            this.LogMessage("CONCAT", result);
        }

        public void Cut(double start, double duration, string inputFile, string outputFile)
        {
            EnsureFileDoesNotExist(outputFile);
            var result = new FFMpegCommandBuilder()
                .InputFrom(inputFile)
                .StartFrom(start)
                .DurationIs(duration)
                .OutputVideoCodec("libx264")
                .OutputPreset(this.presetParameters)
                .OutputTune("fastdecode")
                .WithFlags("+ildct+ilme")
                .OutputTo(outputFile)
                .BuildCommand(this.pathToFfMpegExe)
                .Execute();
            this.LogMessage("CUT", result);
        }

        public void CutAndDrawText(string inputFile, int start, int end, string outputFile, string overlayText)
        {
            EnsureFileDoesNotExist(outputFile);
            if (overlayText == null)
            {
                this.Cut(start, end, inputFile, outputFile);
                return;
            }
            var lastDot = inputFile.LastIndexOf('.');
            var inputExt = inputFile.Substring(lastDot);
            var intermediateFile = GetIntermediateFile(inputExt);
            this.Cut(start, end, inputFile, intermediateFile);
            this.DrawText(intermediateFile, overlayText, outputFile);
            File.Delete(intermediateFile);
        }


        public void CutAndDrawTextAndDrawImage(string inputFile, double start, double end, string outputFile, string overlayText, List<DrawImageTimeRecord> imagesTimeTable, Action<double> progressAction)
        {
            EnsureFileDoesNotExist(outputFile);
            const string ExtensionForResultFile = ".avi";
            var imagesExist = imagesTimeTable != null && imagesTimeTable.Any();
            if (overlayText == null && !imagesExist)
            {
                this.Cut(start, end, inputFile, outputFile);
                progressAction(1);//100% progress
                return;
            }
            var intermediateFile1 = GetIntermediateFile(ExtensionForResultFile);

            this.Cut(start, end, inputFile, intermediateFile1);
            progressAction(0.5);//50% progress

            if (overlayText != null)
            {
                var intermediateFile2 = imagesExist ? GetIntermediateFile(ExtensionForResultFile) : outputFile;
                this.DrawText(intermediateFile1, overlayText, intermediateFile2);
                progressAction(0.75);//75% progress
                File.Delete(intermediateFile1);
                intermediateFile1 = intermediateFile2;
            }
            if (imagesExist)
            {
                this.DrawImage(intermediateFile1, imagesTimeTable, outputFile);
                File.Delete(intermediateFile1);
            }
            progressAction(1);//100% progress
        }

        public void DrawImage(string inputFile, List<DrawImageTimeRecord> imagesTimeTable, string outputFile)
        {
            if (!imagesTimeTable.Any())
                return;
            EnsureFileDoesNotExist(outputFile);
            var result = new FFMpegCommandBuilder()
                .InputFrom(inputFile)
                .DrawImages(imagesTimeTable)
                .OutputVideoCodec("libx264")
                .WithFlags("+ildct+ilme")
                .OutputTo(outputFile)
                .BuildCommand(this.pathToFfMpegExe)
                .Execute();
            this.LogMessage("DrawImage", result);
        }

        public void DrawText(string inputFile, string overlayText, string outputFile)
        {
            EnsureFileDoesNotExist(outputFile);
            const int FontSize = 30;
            const int CharsInLine = 40;
            var lines = GetTranspositionedText(overlayText, CharsInLine);
            var result = new FFMpegCommandBuilder()
                .InputFrom(inputFile)
                .DrawText(lines, this.fontsPath, FontSize)
                .OutputVideoCodec("libx264")
                .OutputTune("fastdecode")
                .OutputPreset(this.presetParameters)
                .WithFlags("+ildct+ilme")
                .OutputTo(outputFile)
                .BuildCommand(this.pathToFfMpegExe)
                .Execute();
            this.LogMessage("DrawText", result);
        }

        public byte[] GetBitmapFromVideoAsByte(string videoFile, double position, FFMpegImageSize imageSize)
        {
            var intermediateFile = GetIntermediateFile(".jpg");
            var quality = imageSize.ToString().ToLower();
            var parameters = string.Format(CultureInfo.InvariantCulture, "-ss {1} -i \"{0}\" -vframes 1 -s {2} \"{3}\"", videoFile, position, quality, intermediateFile);
            var command = new FFMpegCommand(this.pathToFfMpegExe, parameters);
            var result = command.Execute();
            this.LogMessage("ExtractFrame", result);
            try
            {
                using (var fs = new FileStream(intermediateFile, FileMode.Open))
                {
                    var byteImage = new byte[fs.Length];
                    fs.Read(byteImage, 0, (int) fs.Length);
                    return byteImage;
                }
            }
            finally
            {
                File.Delete(intermediateFile);
            }
        }

        public FFMpegVideoInfo GetVideoInfo(string inputFile)
        {
            var command = new FFMpegCommand(this.pathToFfMpegExe, $"-i \"{inputFile}\"");
            var result = command.Execute();
            this.LogMessage("GetVideoInfo", result);
            return ParseVideoInfo(result);
        }

        public void LogMessage(string title, string message)
        {
            var lockTaken = Monitor.TryEnter(LogFile, new TimeSpan(0, 0, 1));
            if (lockTaken)
            {
                File.AppendAllText(LogFile, $"{DateTime.Now}\r\n-----------{title}:--------------\r\n{message}\r\n---------------------------\r\n");
                Monitor.Exit(LogFile);
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