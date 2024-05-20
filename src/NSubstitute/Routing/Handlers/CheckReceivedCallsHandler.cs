using NSubstitute.Core;
using NSubstitute.ReceivedExtensions;

namespace NSubstitute.Routing.Handlers;

public class CheckReceivedCallsHandler(ICallCollection receivedCalls, ICallSpecificationFactory callSpecificationFactory, IReceivedCallsExceptionThrower exceptionThrower, MatchArgs matchArgs, Quantity requiredQuantity) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        var callSpecification = callSpecificationFactory.CreateFrom(call, matchArgs);
        var allCallsToMethodSpec = callSpecificationFactory.CreateFrom(call, MatchArgs.Any);

        var allCalls = receivedCalls.AllCalls().ToList();
        var matchingCalls = allCalls.Where(callSpecification.IsSatisfiedBy).ToList();

        if (!requiredQuantity.Matches(matchingCalls))
        {
            var relatedCalls = allCalls.Where(allCallsToMethodSpec.IsSatisfiedBy).Except(matchingCalls);
            exceptionThrower.Throw(callSpecification, matchingCalls, relatedCalls, requiredQuantity);
        }

        return RouteAction.Continue();
    }
}