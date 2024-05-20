using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class ClearUnusedCallSpecHandler(IPendingSpecification pendingSpecification) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        pendingSpecification.Clear();

        return RouteAction.Continue();
    }
}