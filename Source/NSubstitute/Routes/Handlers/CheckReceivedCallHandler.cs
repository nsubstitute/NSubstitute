using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class CheckReceivedCallHandler : ICallHandler
    {
        private readonly IReceivedCalls _receivedCalls;
        private readonly ICallResults _configuredResults;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public CheckReceivedCallHandler(IReceivedCalls receivedCalls, ICallResults configuredResults, ICallSpecificationFactory callSpecificationFactory)
        {
            _receivedCalls = receivedCalls;
            _configuredResults = configuredResults;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public object Handle(ICall call)
        {
            var callSpecification = _callSpecificationFactory.CreateFrom(call);
            _receivedCalls.ThrowIfCallNotFound(callSpecification);
            return _configuredResults.GetDefaultResultFor(call);
        }
    }
}