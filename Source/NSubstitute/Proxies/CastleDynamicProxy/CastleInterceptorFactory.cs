using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInterceptorFactory
    {
        public virtual IInterceptor CreateForwardingInterceptor(ICallRouter forwardToCallRouter)
        {
            return new CastleForwardingInterceptor(new CastleInvocationMapper(), forwardToCallRouter);
        }
    }
}