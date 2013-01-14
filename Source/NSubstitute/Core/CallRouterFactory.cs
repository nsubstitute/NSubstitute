using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext)
        {
            var substituteState = new SubstituteState(substitutionContext);
            return new CallRouter(substituteState, substitutionContext, new RouteFactory());
        }
    }
}