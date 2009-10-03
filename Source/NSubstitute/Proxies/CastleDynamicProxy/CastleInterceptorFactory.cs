using Castle.Core.Interceptor;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInterceptorFactory
    {
        public virtual IInterceptor CreateForwardingInterceptor(IInvocationHandler forwardToInvocationHandler)
        {
            return default(IInterceptor);
        }
    }
}