using Castle.Core.Interceptor;

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