using NSubstitute.Core;

namespace NSubstitute.Internal.Routing.Handlers;

public class ClearUnusedCallSpecHandler(IPendingSpecification pendingSpecification) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        pendingSpecification.Clear();

        return RouteAction.Continue();
    }
}