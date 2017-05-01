using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext, SubstituteConfig config)
        {
            var substituteState = new SubstituteState(substitutionContext, config);
            return new CallRouter(substituteState, substitutionContext, new RouteFactory());
        }
    }
}