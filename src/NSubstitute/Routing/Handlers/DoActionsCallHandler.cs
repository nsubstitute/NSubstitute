using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

internal sealed class DoActionsCallHandler(ICallActions callActions) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        callActions.InvokeMatchingActions(call);

        return RouteAction.Continue();
    }
}