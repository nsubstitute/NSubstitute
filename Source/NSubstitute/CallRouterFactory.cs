namespace NSubstitute
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext)
        {
            return new CallRouter(new CallStack(), new CallResults(), new ReflectionHelper(), substitutionContext, new CallSpecificationFactory(substitutionContext));
        }
    }
}