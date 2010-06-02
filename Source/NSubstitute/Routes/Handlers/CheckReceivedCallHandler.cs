using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class CheckReceivedCallHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallNotReceivedExceptionThrower _exceptionThrower;

        public CheckReceivedCallHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory, ICallNotReceivedExceptionThrower exceptionThrower)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
            _exceptionThrower = exceptionThrower;
        }

        public object Handle(ICall call)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call);
            if (_receivedCalls.FindMatchingCalls(callSpecification).Any(x => true)) return null;
            _exceptionThrower.Throw(callSpecification);
            return null;
        }
    }
}