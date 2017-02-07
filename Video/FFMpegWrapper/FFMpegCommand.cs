using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FFMpegWrapper
{
    public class FFMpegCommand
    {
        private readonly IList<string> dependentFilesToDelete = new string[0];
        private readonly string pathToFfMpegExe;
        private readonly string command;

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

        public string Execute()
        {
            var fullpath = string.Concat("\"", this.pathToFfMpegExe, "\"");
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
            process.Start();
            process.PriorityClass = ProcessPriorityClass.RealTime;
            var result = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (!process.HasExited)
            {
                process.Kill();
            }

            this.ClearFielsToDelete();

            return $"{process.StartInfo.FileName} {process.StartInfo.Arguments}\r\n{result}";
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