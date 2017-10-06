using System;

namespace FFMpegWrapper
{
    public class FFMpegException : Exception
    {
        public string AllFFMpegOutput { get; }

        public FFMpegException(string message, string allFFMpegOutput) : base(message)
        {
            this.AllFFMpegOutput = allFFMpegOutput;
        }
    }
}