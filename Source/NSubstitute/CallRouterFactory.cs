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
            
            return new CallRouter(substitutionContext, resultSetter, 
                                    new RouteFactory(
                                        callStack, callResults, reflectionHelper, callSpecificationFactory, resultSetter));
        }
    }
}