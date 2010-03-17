namespace NSubstitute
{
    public class CallHandlerFactory : ICallHandlerFactory
    {
        public ICallHandler CreateCallHandler(ISubstitutionContext substitutionContext)
        {
            var matcher = new AllCallMatcher(new ICallMatcher[] {CreateMethodInfoMatcher(), CreateArgumentsEqualMatcher()});
            return new CallHandler(new CallStack(matcher), new CallResults(matcher), substitutionContext);
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