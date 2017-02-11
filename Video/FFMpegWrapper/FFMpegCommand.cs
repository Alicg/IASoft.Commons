using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace FFMpegWrapper
{
    public class FFMpegCommand
    {
        private readonly IList<string> dependentFilesToDelete = new string[0];
        private readonly Action<double> progressCallback;
        private readonly string pathToFfMpegExe;
        private readonly string command;

        private TimeSpan commandDuration = TimeSpan.Zero;

        public FFMpegCommand(string pathToFfMpegExe, string command)
        {
            this.pathToFfMpegExe = pathToFfMpegExe;
            this.command = command;
        }

        public FFMpegCommand(string pathToFfMpegExe, string command, IList<string> dependentFilesToDelete)
            : this(pathToFfMpegExe, command)
        {
            this.dependentFilesToDelete = dependentFilesToDelete;
        }

        public FFMpegCommand(string pathToFfMpegExe, string command, IList<string> dependentFilesToDelete, Action<double> progressCallback)
            : this(pathToFfMpegExe, command)
        {
            this.dependentFilesToDelete = dependentFilesToDelete;
            this.progressCallback = progressCallback;
        }

        public string Execute()
        {
            var fullpath = string.Concat("\"", this.pathToFfMpegExe, "\"");
            var fullLog = string.Empty;
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = fullpath,
                    Arguments = this.command,
                    Verb = "runas"
                }
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                {
                    return;
                }
                fullLog += args.Data;
                this.NotifyProgressChanged(args.Data);
            };
            process.Start();
            process.PriorityClass = ProcessPriorityClass.RealTime;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            if (!process.HasExited)
            {
                process.Kill();
            }

            this.ClearFielsToDelete();

            return $"{process.StartInfo.FileName} {process.StartInfo.Arguments}\r\n{fullLog}";
        }

        private void NotifyProgressChanged(string ffmpegProgressLog)
        {
            if (this.progressCallback == null)
            {
                return;
            }
            var durationRegex = new Regex("Duration:\\s(?<duration>[0-9:.]+)([,]|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            var progressRegex = new Regex("time=(?<progress>[0-9:.]+)\\s", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            var durationMatch = durationRegex.Match(ffmpegProgressLog);
            if (durationMatch.Success)
            {
                TimeSpan duration;
                TimeSpan.TryParse(durationMatch.Groups["duration"].Value, out duration);
                this.commandDuration += duration;
            }
            var progressMatch = progressRegex.Match(ffmpegProgressLog);
            if (progressMatch.Success && this.commandDuration != TimeSpan.Zero)
            {
                TimeSpan progressInSeconds;
                if (TimeSpan.TryParse(progressMatch.Groups["progress"].Value, out progressInSeconds))
                {
                    double currentProgress;
                    if (progressInSeconds > this.commandDuration)
                    {
                        currentProgress = 1;
                    }
                    else
                    {
                        currentProgress = progressInSeconds.TotalSeconds / this.commandDuration.TotalSeconds;
                    }
                    this.progressCallback(currentProgress);
                }
            }
        }

        private void ClearFielsToDelete()
        {
            foreach (var fileToDelete in this.dependentFilesToDelete)
            {
                if (File.Exists(fileToDelete))
                {
                    File.Delete(fileToDelete);
                }
            }
        }
    }
}