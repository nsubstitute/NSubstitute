using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class ReturnResultForTypeHandler(IResultsForType resultsForType) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        if (resultsForType.TryGetResult(call, out var result))
        {
            return RouteAction.Return(result);
        }

        return RouteAction.Continue();
    }
}