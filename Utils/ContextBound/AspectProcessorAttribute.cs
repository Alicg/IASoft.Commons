using System;
using System.Runtime.Remoting.Messaging;

namespace Utils.ContextBound
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = true)]
    public abstract class AspectProcessorAttribute : Attribute
    {
        public abstract void PreProcess(ref IMethodCallMessage msg);
        public abstract void PostProcess(IMethodCallMessage callMsg, ref IMethodReturnMessage retMsg);
    }
}