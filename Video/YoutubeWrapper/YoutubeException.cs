using System;

namespace YoutubeWrapper
{
    public class YoutubeException : Exception
    {
        public YoutubeException(string message) : base(message)
        {
        }

        public YoutubeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}