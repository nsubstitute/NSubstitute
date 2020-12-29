using System;
using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleForwardingInterceptor : IInterceptor
    {
        private readonly CastleInvocationMapper _invocationMapper;
        private readonly ICallRouter _callRouter;
        private bool _fullDispatchMode;

        public CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallRouter callRouter)
        {
            _invocationMapper = invocationMapper;
            _callRouter = callRouter;
        }

        public void Intercept(IInvocation invocation)
        {
            ICall mappedInvocation = _invocationMapper.Map(invocation);

            if (_fullDispatchMode)
            {
                invocation.ReturnValue = _callRouter.Route(mappedInvocation);
                return;
            }

            // Fallback to the base value until the full dispatch mode is activated.
            // Useful to ensure that object is initialized properly.
            if (_callRouter.CallBaseByDefault)
            {
                invocation.ReturnValue = mappedInvocation.TryCallBase().ValueOrDefault();
            }
        }

        /// <summary>
        /// Switches interceptor to dispatch calls via the full pipeline.
        /// </summary>
        public void SwitchToFullDispatchMode()
        {
            _fullDispatchMode = true;
        }
    }
}