using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class TrackLastCallHandler(IPendingSpecification pendingSpecification) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        pendingSpecification.SetLastCall(call);

        return RouteAction.Continue();
    }
}