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
        private readonly bool _withAnyArgs;

        public CheckDidNotReceiveCallHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallReceivedExceptionThrower exceptionThrower, bool withAnyArgs)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
            _withAnyArgs = withAnyArgs;
        }

        public object Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, _withAnyArgs);
            if (_receivedCalls.FindMatchingCalls(callSpec).Any()) _exceptionThrower.Throw(callSpec);
            return null;
        }
    }
}