namespace NSubstitute
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext)
        {
            var substituteState = new SubstituteState();
            return new CallRouter(substitutionContext, substituteState.ResultSetter, new RouteFactory(substituteState));
        }
    }
}