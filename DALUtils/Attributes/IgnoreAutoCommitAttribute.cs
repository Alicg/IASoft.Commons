namespace Utils.DAL.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class IgnoreAutoCommitAttribute : Attribute { }
}
