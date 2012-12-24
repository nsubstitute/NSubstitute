using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        readonly CastleInvocationMapper _invocationMapper;
        readonly ICallRouter _callRouter;
        private bool _isIntercepting;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallRouter callRouter)
        {
            _invocationMapper = invocationMapper;
            _callRouter = callRouter;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!_isIntercepting) return;
            var mappedInvocation = _invocationMapper.Map(invocation);
            var returnValue = _callRouter.Route(mappedInvocation);
            if (returnValue == mappedInvocation.CallOriginalMethod())
                invocation.Proceed();
            else
                invocation.ReturnValue = returnValue;
        }

        public void StartIntercepting() { _isIntercepting = true; }
    }
}