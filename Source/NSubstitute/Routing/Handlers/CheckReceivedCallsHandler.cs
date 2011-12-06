using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class CheckReceivedCallsHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly IReceivedCallsExceptionThrower _exceptionThrower;
        private readonly MatchArgs _matchArgs;
        private readonly Quantity _requiredQuantity;

        public CheckReceivedCallsHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, IReceivedCallsExceptionThrower exceptionThrower, MatchArgs matchArgs, Quantity requiredQuantity)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
            _matchArgs = matchArgs;
            _requiredQuantity = requiredQuantity;
        }

        public RouteAction Handle(ICall call)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            var allCallsToMethodSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.Any);

            var allCalls = _receivedCalls.AllCalls();
            var matchingCalls = allCalls.Where(x => callSpecification.IsSatisfiedBy(x));
            var relatedCalls = allCalls.Where(x => allCallsToMethodSpec.IsSatisfiedBy(x)).Except(matchingCalls);

            if (!_requiredQuantity.Matches(matchingCalls))
            {
                _exceptionThrower.Throw(callSpecification, matchingCalls, relatedCalls, _requiredQuantity);
            }
            return RouteAction.Continue();
        }
    }
}