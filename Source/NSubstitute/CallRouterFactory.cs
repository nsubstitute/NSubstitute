namespace NSubstitute
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext)
        {
            var substituteState = new SubstituteState(substitutionContext);
            var routePartsFactory = new RoutePartsFactory(substituteState);
            return new CallRouter(substitutionContext, substituteState.ResultSetter, new RouteFactory(routePartsFactory));
        }
    }
}