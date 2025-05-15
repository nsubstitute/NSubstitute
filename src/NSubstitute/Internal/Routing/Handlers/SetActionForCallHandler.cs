using NSubstitute.Core;
using NSubstitute.Internal.Core;

namespace NSubstitute.Internal.Routing.Handlers;

public class SetActionForCallHandler(
    ICallSpecificationFactory callSpecificationFactory,
    ICallActions callActions,
    Action<CallInfo> action,
    MatchArgs matchArgs) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        var callSpec = callSpecificationFactory.CreateFrom(call, matchArgs);
        callActions.Add(callSpec, action);

        return RouteAction.Continue();
    }
}