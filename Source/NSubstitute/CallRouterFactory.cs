namespace NSubstitute
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext)
        {
            var callStack = new CallStack();
            var callResults = new CallResults();
            var reflectionHelper = new ReflectionHelper();
            var callSpecificationFactory = new CallSpecificationFactory(substitutionContext);
            var resultSetter = new ResultSetter(callStack, callResults, callSpecificationFactory);
            var recordingCallHandler = new RecordCallHandler(callStack, callResults);
            var checkReceivedCallHandler = new CheckReceivedCallHandler(callStack, callResults, callSpecificationFactory);
            var propertySetterHandler = new PropertySetterHandler(reflectionHelper, resultSetter);
            var eventSubscriptionHandler = new EventSubscriptionHandler();
            
            return new CallRouter(substitutionContext, recordingCallHandler, 
                                    propertySetterHandler, eventSubscriptionHandler, 
                                    checkReceivedCallHandler, resultSetter);
        }
    }
}