using System;

namespace Utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, Inherited = false,
        AllowMultiple = false)]
    public sealed class LoggableAttribute : Attribute
    {
    }
}