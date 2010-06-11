namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext)
        {
            var substituteState = new SubstituteState(substitutionContext);
            var routePartsFactory = new RoutePartsFactory(substituteState);
            return new CallRouter(substitutionContext, substituteState.CallStack, substituteState.ResultSetter, new RouteFactory(routePartsFactory));
        }
    }
}