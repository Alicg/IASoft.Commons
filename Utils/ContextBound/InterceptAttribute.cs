using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace Utils.ContextBound
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InterceptAttribute : ContextAttribute
    {
        public InterceptAttribute()
            : base("Intercept")
        {
        }

        public override void Freeze(Context newContext)
        {
        }

        public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
        {
            ctorMsg.ContextProperties.Add(new InterceptProperty());
        }

        public override bool IsContextOK(Context ctx, IConstructionCallMessage ctorMsg)
        {
            var p = ctx.GetProperty("Intercept") as InterceptProperty;
            return p != null;
        }

        public override bool IsNewContextOK(Context newCtx)
        {
            var p = newCtx.GetProperty("Intercept") as InterceptProperty;
            return p != null;
        }
    }
}