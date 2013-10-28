using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext, bool callBaseByDefault)
        {
            var substituteState = new SubstituteState(substitutionContext, callBaseByDefault);
            return new CallRouter(substituteState, substitutionContext, new RouteFactory());
        }
    }
}