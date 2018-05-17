using System;

namespace YoutubeWrapper
{
    public class GoogleException : Exception
    {
        public GoogleException(string message) : base(message)
        {
        }

        public GoogleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}