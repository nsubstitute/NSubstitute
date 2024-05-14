using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class ReturnDefaultForReturnTypeHandler(IDefaultForType defaultForType) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        var returnValue = defaultForType.GetDefaultFor(call.GetMethodInfo().ReturnType);
        return RouteAction.Return(returnValue);
    }
}