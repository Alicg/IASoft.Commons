using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FFMpegExecutable;

namespace FFMpegWrapper
{
    public class FFMpegCommand
    {
        private readonly bool ignoreError;
        private readonly Action<double, double, int> progressCallback;
        private readonly IObservable<double> stopSignal;
        private readonly ProcessPriorityClass processPriorityClass;
        private readonly string pathToFfMpegExe;
        private readonly string command;

        private TimeSpan commandDuration = TimeSpan.Zero;

        public FFMpegCommand(string pathToFfMpegExe, string command, bool ignoreError = false)
            : this(pathToFfMpegExe, command, null, Observable.Empty<double>(), ProcessPriorityClass.High, ignoreError)
        {
        }

        public FFMpegCommand(
            string pathToFfMpegExe,
            string command,
            Action<double, double, int> progressCallback,
            IObservable<double> stopSignal,
            ProcessPriorityClass processPriorityClass,
            bool ignoreError = false)
        {
            this.pathToFfMpegExe = pathToFfMpegExe;
            this.command = command;
            this.progressCallback = progressCallback;
            this.stopSignal = stopSignal;
            this.processPriorityClass = processPriorityClass;
            this.ignoreError = ignoreError;
        }
        
        public int ProcessId { get; private set; }

        public string Execute()
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
            process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data == null)
                    {
                        return;
                    }
                    fullLogBuilder.AppendLine($"{DateTime.Now.ToLongTimeString()} {args.Data}");
                    if (this.progressCallback != null)
                    {
                        this.NotifyProgressChanged(args.Data, process.Id);
                    }
                };

            bool cancelled = false;
            using (this.stopSignal.Subscribe(
                v =>
                    {
                        process.Kill();
                        cancelled = true;
                    }))
            {
                process.Start();
                try
                {
                    this.ProcessId = process.Id;
                    process.PriorityClass = this.processPriorityClass;
                }
                catch
                {
                    // ignored
                }
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

            var fullLog = fullLogBuilder.ToString();
            if (string.IsNullOrEmpty(fullLog))
            {
                fullLog = process.StandardError.ReadToEnd();
            }

            if (process.ExitCode != 0 && !this.ignoreError)
            {
                if (cancelled)
                {
                    throw new FFMpegCancelledException("Process was cancelled", $"{this.command}\r\n{fullLog}");
                }
                var lastFFMpegOutput = fullLog.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                throw new FFMpegException(lastFFMpegOutput, $"{this.command}\r\n{fullLog}");
            }

            return $"{process.StartInfo.FileName} {process.StartInfo.Arguments}\r\n{fullLog}";
        }

        private void NotifyProgressChanged(string ffmpegProgressLog, int processId)
        {
            var durationRegex = new Regex("Duration:\\s(?<duration>[0-9:.]+)([,]|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            var progressRegex = new Regex("time=(?<progress>[0-9:.]+)\\s", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            var durationMatch = durationRegex.Match(ffmpegProgressLog);
            if (durationMatch.Success)
            {
                TimeSpan duration;
                TimeSpan.TryParse(durationMatch.Groups["duration"].Value, out duration);
                
                //TODO: это некорректная логика, т.к. в duration будет длительность самого видео, а не длительность его обработки.
                this.commandDuration += duration;
            }
            var progressMatch = progressRegex.Match(ffmpegProgressLog);
            if (progressMatch.Success && this.commandDuration != TimeSpan.Zero)
            {
                TimeSpan progressInSeconds;
                if (TimeSpan.TryParse(progressMatch.Groups["progress"].Value, out progressInSeconds))
                {
                    this.progressCallback(progressInSeconds.TotalSeconds, this.commandDuration.TotalSeconds, processId);
                }
            }
        }
    }
}