using System.Collections.Generic;
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
            var matchingCalls = _receivedCalls.FindMatchingCalls(callSpecification);

            if (!_requiredQuantity.Matches(matchingCalls))
            {
                _exceptionThrower.Throw(callSpecification, matchingCalls, GetAllRelatedCallsToMethod(call).Except(matchingCalls), _requiredQuantity);
            }
            return RouteAction.Continue();
        }

        private IEnumerable<ICall> GetAllRelatedCallsToMethod(ICall call)
        {
            var allCallsToMethodSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.Any);
            return _receivedCalls.FindMatchingCalls(allCallsToMethodSpec);
        }
    }
}