using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

internal sealed class ReturnFromBaseIfRequired(ICallBaseConfiguration config) : ICallHandler
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