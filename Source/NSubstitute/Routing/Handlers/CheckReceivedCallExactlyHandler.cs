using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class CheckReceivedCallExactlyHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallNotReceivedExactlyExceptionThrower _exceptionThrower;
        private readonly MatchArgs _matchArgs;
        private readonly int _exceptedCount;

        public CheckReceivedCallExactlyHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallNotReceivedExactlyExceptionThrower exceptionThrower, MatchArgs matchArgs, int exceptedCount)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
            _matchArgs = matchArgs;
            _exceptedCount = exceptedCount;
        }

        public RouteAction Handle(ICall call)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            int actualCount = _receivedCalls.FindMatchingCalls(callSpecification).Count();
            if (actualCount != _exceptedCount)
            {
                _exceptionThrower.Throw(callSpecification, GetAllReceivedCallsToMethod(call), _exceptedCount);
            }
            return RouteAction.Continue();
        }

        private IEnumerable<ICall> GetAllReceivedCallsToMethod(ICall call)
        {
            var allCallsToMethodSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.Any);
            return _receivedCalls.FindMatchingCalls(allCallsToMethodSpec);
        }
    }
}