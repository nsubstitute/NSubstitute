using Castle.Core.Interceptor;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleInterceptorFactory
    {
        public virtual IInterceptor CreateForwardingInterceptor(ICallHandler forwardToCallHandler)
        {
            return new CastleForwardingInterceptor(new CastleInvocationMapper(), forwardToCallHandler);
        }
    }
}