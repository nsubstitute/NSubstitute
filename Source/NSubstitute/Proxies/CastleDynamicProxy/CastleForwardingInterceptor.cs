using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        readonly CastleInvocationMapper _invocationMapper;
        readonly ICallRouter CallRouter;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallRouter callRouter)
        {
            _invocationMapper = invocationMapper;
            CallRouter = callRouter;
        }

        public void Intercept(IInvocation invocation)
        {
            var mappedInvocation = _invocationMapper.Map(invocation);
            invocation.ReturnValue = CallRouter.Route(mappedInvocation);
        }
    }
}