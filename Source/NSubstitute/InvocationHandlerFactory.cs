namespace NSubstitute
{
    public class InvocationHandlerFactory : IInvocationHandlerFactory
    {
        public IInvocationHandler CreateInvocationHandler(ISubstitutionContext substitutionContext)
        {
            var matcher = new AllInvocationMatcher(new IInvocationMatcher[] {CreateMethodInfoMatcher(), CreateArgumentsEqualMatcher()});
            return new InvocationHandler(new InvocationStack(), new InvocationResults(matcher), substitutionContext);
        }

        private MethodInfoMatcher CreateMethodInfoMatcher()
        {
            return new MethodInfoMatcher();
        }

        private ArgumentsEqualMatcher CreateArgumentsEqualMatcher()
        {
            return new ArgumentsEqualMatcher(new ArgumentEqualityComparer());
        }
    }
}