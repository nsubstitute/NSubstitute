using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext, ISubstituteState substituteState)
        {
            return new CallRouter(substituteState, substitutionContext, new RouteFactory());
        }
    }
}