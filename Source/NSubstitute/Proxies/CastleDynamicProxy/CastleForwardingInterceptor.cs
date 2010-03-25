using System.Collections.Generic;
using Castle.Core.Interceptor;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        readonly CastleInvocationMapper _invocationMapper;
        readonly ICallHandler callHandler;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallHandler callHandler)
        {
            _invocationMapper = invocationMapper;
            this.callHandler = callHandler;
        }

        public void Intercept(Castle.Core.Interceptor.IInvocation invocation)
        {
            var mappedInvocation = _invocationMapper.Map(invocation);
            invocation.ReturnValue = callHandler.Handle(mappedInvocation, new List<IArgumentMatcher>());
        }
    }
}