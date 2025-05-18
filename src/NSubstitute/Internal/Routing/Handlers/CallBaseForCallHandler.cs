using NSubstitute.Internal.Core;
using NSubstitute.Exceptions;
using NSubstitute.Core;

namespace NSubstitute.Internal.Routing.Handlers;

public class CallBaseForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallBaseConfiguration callBaseConfig, MatchArgs matchArgs) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        if (!call.CanCallBase) throw CouldNotConfigureCallBaseException.ForSingleCall();

        var callSpec = callSpecificationFactory.CreateFrom(call, matchArgs);
        callBaseConfig.Include(callSpec);

        return RouteAction.Continue();
    }
}