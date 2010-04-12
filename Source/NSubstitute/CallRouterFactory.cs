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
            var recordingCallHandler = new RecordCallHandler(callStack, callResults, reflectionHelper, callSpecificationFactory);
            var checkReceivedCallHandler = new CheckReceivedCallHandler(callStack, callResults, callSpecificationFactory);
            var resultSetter = new ResultSetter(callStack, callResults, callSpecificationFactory);
            
            return new CallRouter(reflectionHelper, substitutionContext, recordingCallHandler, checkReceivedCallHandler, resultSetter);
        }
    }
}