namespace NSubstitute
{
    public class InvocationHandlerFactory : IInvocationHandlerFactory
    {
        public IInvocationHandler CreateInvocationHandler(ISubstitutionContext substitutionContext)
        {
            var matcher = new AllInvocationMatcher(new[] {new MethodInfoMatcher()});
            return new InvocationHandler(new InvocationStack(), new InvocationResults(matcher), substitutionContext);
        }
    }
}