using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class CheckReceivedCallHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public CheckReceivedCallHandler(IReceivedCalls receivedCalls, ICallSpecificationFactory callSpecificationFactory)
        {
            _receivedCalls = receivedCalls;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public object Handle(ICall call)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call);
            _receivedCalls.ThrowIfCallNotFound(callSpecification);
            return null;
        }
    }
}