using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class DoActionsCallHandler(ICallActions callActions) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        callActions.InvokeMatchingActions(call);

        return RouteAction.Continue();
    }
}