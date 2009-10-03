using Castle.Core.Interceptor;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        readonly CastleInvocationMapper _invocationMapper;
        readonly IInvocationHandler _invocationHandler;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, IInvocationHandler invocationHandler)
        {
            _invocationMapper = invocationMapper;
            _invocationHandler = invocationHandler;
        }

        public void Intercept(Castle.Core.Interceptor.IInvocation invocation)
        {
            var mappedInvocation = _invocationMapper.Map(invocation);
            _invocationHandler.HandleInvocation(mappedInvocation);
        }
    }
}