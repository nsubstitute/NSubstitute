using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class CheckReceivedCallHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallNotReceivedExceptionThrower _exceptionThrower;
        private readonly MatchArgs _matchArgs;

        public CheckReceivedCallHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallNotReceivedExceptionThrower exceptionThrower, MatchArgs matchArgs)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
            _matchArgs = matchArgs;
        }

        public object Handle(ICall call)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            if (_receivedCalls.FindMatchingCalls(callSpecification).Any()) return null;
            _exceptionThrower.Throw(callSpecification, GetAllReceivedCallsToMethod(call));
            return null;
        }

        private IEnumerable<ICall> GetAllReceivedCallsToMethod(ICall call)
        {
            var allCallsToMethodSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.Any);
            return _receivedCalls.FindMatchingCalls(allCallsToMethodSpec);
        }
    }
}