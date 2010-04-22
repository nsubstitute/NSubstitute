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
            var eventHandlerRegistry = new EventHandlerRegistry();
            var eventSubscriptionHandler = new EventSubscriptionHandler(eventHandlerRegistry);
            var eventRaiser = new EventRaiser(eventHandlerRegistry);

            return new CallRouter(substitutionContext, recordingCallHandler, 
                                    propertySetterHandler, eventSubscriptionHandler, 
                                    checkReceivedCallHandler, resultSetter, eventRaiser);
        }
    }
}