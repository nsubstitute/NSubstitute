using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class CheckReceivedCallHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallNotReceivedExceptionThrower _exceptionThrower;
        private readonly bool _withAnyArgs;

        public CheckReceivedCallHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallNotReceivedExceptionThrower exceptionThrower, bool withAnyArgs)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
            _withAnyArgs = withAnyArgs;
        }

        public object Handle(ICall call)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call, _withAnyArgs);
            if (_receivedCalls.FindMatchingCalls(callSpecification).Any()) return null;
            _exceptionThrower.Throw(callSpecification);
            return null;
        }
    }
}