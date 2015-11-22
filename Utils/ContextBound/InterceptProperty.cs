using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Utils.Extensions;

namespace Utils.ContextBound
{
    public class InterceptProperty : IContextProperty, IContributeObjectSink
    {
        #region IContextProperty Members

        public string Name
        {
            get { return "Intercept"; }
        }

        public bool IsNewContextOK(Context newCtx)
        {
            var p = newCtx.GetProperty("Intercept") as InterceptProperty;
            return p != null;
        }

        public void Freeze(Context newContext)
        {
        }

        #endregion

        #region IContributeObjectSink Members

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            //var dd = ((System.Runtime.Remoting.Proxies.RemotingProxy)(((System.Runtime.Remoting.Proxies.__TransparentProxy)(((System.Runtime.Remoting.Proxies.__TransparentProxy)(obj))))._rp)).TypeName;
            RealProxy proxy = RemotingServices.GetRealProxy(obj);
            var type = proxy.InvokeByName<Type>("GetProxiedType");
            return new InterceptSink(nextSink, type);
        }

        #endregion
    }
}