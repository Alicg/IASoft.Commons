using System;
using System.Runtime.Serialization;

namespace Utils
{
    [Serializable]
    public class ResourceUnpackException : Exception
    {
        public ResourceUnpackException()
        {
        }

        public ResourceUnpackException(string message) : base(message)
        {
        }

        public ResourceUnpackException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ResourceUnpackException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}