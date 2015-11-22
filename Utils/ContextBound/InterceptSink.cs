using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Utils.Extensions;

namespace Utils.ContextBound
{
    public class InterceptSink : IMessageSink
    {
        private readonly IMessageSink nextSink;
        private readonly Type sourceType;

        public InterceptSink(IMessageSink nextSink, Type sourceType)
        {
            this.nextSink = nextSink;
            this.sourceType = sourceType;
        }

        #region IMessageSink Members

        public IMessage SyncProcessMessage(IMessage msg)
        {
            var mcm = (msg as IMethodCallMessage);
            PreProcess(ref mcm);
            IMessage rtnMsg = nextSink.SyncProcessMessage(msg);
            var mrm = (rtnMsg as IMethodReturnMessage);
            PostProcess(msg as IMethodCallMessage, ref mrm);
            return mrm;
        }

        public IMessageSink NextSink
        {
            get { return nextSink; }
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            IMessageCtrl rtnMsgCtrl = nextSink.AsyncProcessMessage(msg, replySink);
            return rtnMsgCtrl;
        }

        #endregion

        private void PreProcess(ref IMethodCallMessage callMsg)
        {
            string methodName = callMsg.MethodName;
            MethodInfo method = sourceType.GetMethods().Where(a => a.Name == methodName).FirstOrDefault();
            if (method.IsNull()) return;
            object[] attrs = method.GetCustomAttributes(typeof (AspectProcessorAttribute), true);
            foreach (AspectProcessorAttribute a in attrs)
                a.PreProcess(ref callMsg);
        }

        private void PostProcess(IMethodCallMessage callMsg, ref IMethodReturnMessage rtnMsg)
        {
            string methodName = callMsg.MethodName;
            MethodInfo method = sourceType.GetMethods().Where(a => a.Name == methodName).FirstOrDefault();
            if (method.IsNull()) return;
            object[] attrs = method.GetCustomAttributes(typeof (AspectProcessorAttribute), true);
            foreach (AspectProcessorAttribute a in attrs)
                a.PostProcess(callMsg, ref rtnMsg);
        }
    }
}