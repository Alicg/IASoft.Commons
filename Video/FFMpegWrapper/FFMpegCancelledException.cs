using System;
using FFMpegExecutable;

namespace FFMpegWrapper
{
    public class FFMpegCancelledException : FFMpegException
    {
         public FFMpegCancelledException(string message, string allFFMpegOutput) : base(message, allFFMpegOutput) { }
    }
}