using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FFMpegWrapper
{
    public class FFMpegCommand
    {
        private readonly bool ignoreError;
        private readonly IList<string> dependentFilesToDelete;
        private readonly Action<double> progressCallback;
        private readonly IObservable<double> stopSignal;
        private readonly string pathToFfMpegExe;
        private readonly string command;

        private TimeSpan commandDuration = TimeSpan.Zero;

        public FFMpegCommand(string pathToFfMpegExe, string command, bool ignoreError = false)
            : this(pathToFfMpegExe, command, new string[0])
        {
            this.ignoreError = ignoreError;
        }

        public FFMpegCommand(string pathToFfMpegExe, string command, IList<string> dependentFilesToDelete, bool ignoreError = false)
            : this(pathToFfMpegExe, command, dependentFilesToDelete, null, Observable.Empty<double>(), ignoreError)
        {
        }

        public FFMpegCommand(
            string pathToFfMpegExe,
            string command,
            IList<string> dependentFilesToDelete,
            Action<double> progressCallback,
            IObservable<double> stopSignal,
            bool ignoreError = false)
        {
            this.pathToFfMpegExe = pathToFfMpegExe;
            this.command = command;
            this.dependentFilesToDelete = dependentFilesToDelete;
            this.progressCallback = progressCallback;
            this.stopSignal = stopSignal;
            this.ignoreError = ignoreError;
        }

        public string Execute()
        {
            try
            {
                var fullpath = string.Concat("\"", this.pathToFfMpegExe, "\"");
                var fullLogBuilder = new StringBuilder();
                var process = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = fullpath,
                        Arguments = this.command,
                        Verb = "runas"
                    }
                };
                if (this.progressCallback != null)
                {
                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data == null)
                        {
                            return;
                        }
                        fullLogBuilder.AppendLine(args.Data);
                        this.NotifyProgressChanged(args.Data);
                    };
                }

                using (this.stopSignal.Subscribe(v => process.Kill()))
                {
                    process.Start();
                    process.PriorityClass = ProcessPriorityClass.High;
                    if (this.progressCallback != null)
                    {
                        process.BeginErrorReadLine();
                    }
                    process.WaitForExit();
                }

                if (!process.HasExited)
                {
                    process.Kill();
                }

                var fullLog = this.progressCallback != null ? fullLogBuilder.ToString() : process.StandardError.ReadToEnd();

                if (process.ExitCode != 0 && !this.ignoreError)
                {
                    var lastFFMpegOutput = fullLog.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                    throw new FFMpegException(lastFFMpegOutput);
                }

                return $"{process.StartInfo.FileName} {process.StartInfo.Arguments}\r\n{fullLog}";
            }
            finally
            {
                this.ClearFielsToDelete();
            }
        }

        private void NotifyProgressChanged(string ffmpegProgressLog)
        {
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
                        // FFMpeg может долго отдавать нулевой прогресс, но если операция началась будем отдавать хотя бы о 10% текущей операции.
                        currentProgress = Math.Max(0.1, progressInSeconds.TotalSeconds / this.commandDuration.TotalSeconds);
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