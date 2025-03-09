using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Routing.Handlers;

internal sealed class CallBaseForCallHandler(
    ICallSpecificationFactory callSpecificationFactory,
    ICallBaseConfiguration callBaseConfig,
    MatchArgs matchArgs) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        if (!call.CanCallBase) throw CouldNotConfigureCallBaseException.ForSingleCall();

        var callSpec = callSpecificationFactory.CreateFrom(call, matchArgs);
        callBaseConfig.Include(callSpec);

        return RouteAction.Continue();
    }
}