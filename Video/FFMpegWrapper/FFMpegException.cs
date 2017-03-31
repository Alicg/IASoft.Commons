using System;

namespace FFMpegWrapper
{
    public class FFMpegException : Exception
    {
         public FFMpegException(string message) : base(message) { }
    }
}