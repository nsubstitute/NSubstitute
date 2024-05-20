using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class ReturnFromBaseIfRequired(ICallBaseConfiguration config) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        if (config.ShouldCallBase(call))
        {
            return call
                .TryCallBase()
                .Fold(RouteAction.Continue, RouteAction.Return);
        }

        return RouteAction.Continue();
    }
}