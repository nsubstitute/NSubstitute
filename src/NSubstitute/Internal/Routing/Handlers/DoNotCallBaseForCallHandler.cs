using NSubstitute.Internal.Core;
using NSubstitute.Exceptions;
using NSubstitute.Core;

namespace NSubstitute.Internal.Routing.Handlers;

public class DoNotCallBaseForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallBaseConfiguration callBaseConfig, MatchArgs matchArgs) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        if (!call.CanCallBase) throw CouldNotConfigureCallBaseException.ForSingleCall();

        var callSpec = callSpecificationFactory.CreateFrom(call, matchArgs);
        callBaseConfig.Exclude(callSpec);

        return RouteAction.Continue();
    }
}