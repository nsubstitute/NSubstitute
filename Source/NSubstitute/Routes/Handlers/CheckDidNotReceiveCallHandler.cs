using System;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class CheckDidNotReceiveCallHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallReceivedExceptionThrower _exceptionThrower;

        public CheckDidNotReceiveCallHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallReceivedExceptionThrower exceptionThrower)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
        }

        public object Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, false);
            if (_receivedCalls.FindMatchingCalls(callSpec).Any()) _exceptionThrower.Throw(callSpec);
            return null;
        }
    }
}