using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class CheckDidNotReceiveCallHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallReceivedExceptionThrower _exceptionThrower;
        private readonly MatchArgs _matchArgs;

        public CheckDidNotReceiveCallHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallReceivedExceptionThrower exceptionThrower, MatchArgs matchArgs)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
            _matchArgs = matchArgs;
        }

        public object Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            if (_receivedCalls.FindMatchingCalls(callSpec).Any()) _exceptionThrower.Throw(callSpec);
            return null;
        }
    }
}