using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class RecordCallSpecificationHandler(IPendingSpecification pendingCallSpecification, ICallSpecificationFactory callSpecificationFactory, ICallActions callActions) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        var callSpec = callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
        pendingCallSpecification.SetCallSpecification(callSpec);

        // Performance optimization - don't register call actions if current argument matchers
        // don't have any callbacks.
        if (call.GetArgumentSpecifications().Any(x => x.HasAction))
        {
            callActions.Add(callSpec);
        }

        return RouteAction.Continue();
    }
}