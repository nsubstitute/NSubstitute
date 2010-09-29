using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        readonly CastleInvocationMapper _invocationMapper;
        readonly ICallRouter _callRouter;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallRouter callRouter)
        {
            _invocationMapper = invocationMapper;
            _callRouter = callRouter;
        }

        public void Intercept(IInvocation invocation)
        {
            var mappedInvocation = _invocationMapper.Map(invocation);
            invocation.ReturnValue = _callRouter.Route(mappedInvocation);
        }
    }
}