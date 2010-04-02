using Castle.Core.Interceptor;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        readonly CastleInvocationMapper _invocationMapper;
        readonly ICallHandler _callHandler;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallHandler callHandler)
        {
            _invocationMapper = invocationMapper;
            _callHandler = callHandler;
        }

        public void Intercept(IInvocation invocation)
        {
            var mappedInvocation = _invocationMapper.Map(invocation);
            invocation.ReturnValue = _callHandler.Handle(mappedInvocation);
        }
    }
}