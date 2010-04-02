namespace NSubstitute
{
    public class CallHandlerFactory : ICallHandlerFactory
    {
        public ICallHandler CreateCallHandler(ISubstitutionContext substitutionContext)
        {
            return new CallHandler(new CallStack(), new CallResults(), new ReflectionHelper(), substitutionContext, new CallSpecificationFactory(substitutionContext));
        }
    }
}