using Castle.Core.Interceptor;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInterceptorFactory
    {
        public virtual IInterceptor CreateForwardingInterceptor(IInvocationHandler forwardToInvocationHandler)
        {
            return new CastleForwardingInterceptor(new CastleInvocationMapper(), forwardToInvocationHandler);
        }
    }
}