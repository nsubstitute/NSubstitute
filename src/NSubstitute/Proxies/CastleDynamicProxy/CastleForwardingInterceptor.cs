using Castle.DynamicProxy;
using NSubstitute.Core;

namespace NSubstitute.Proxies.CastleDynamicProxy;

public class CastleForwardingInterceptor(CastleInvocationMapper invocationMapper, ICallRouter callRouter) : IInterceptor
{
    private bool _fullDispatchMode;

    public void Intercept(IInvocation invocation)
    {
        ICall mappedInvocation = invocationMapper.Map(invocation);

        if (_fullDispatchMode)
        {
            invocation.ReturnValue = callRouter.Route(mappedInvocation);
            return;
        }

        // Fallback to the base value until the full dispatch mode is activated.
        // Useful to ensure that object is initialized properly.
        if (callRouter.CallBaseByDefault)
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