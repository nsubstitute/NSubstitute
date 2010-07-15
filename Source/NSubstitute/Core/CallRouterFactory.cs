using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        public ICallRouter Create(ISubstitutionContext substitutionContext)
        {
            var substituteState = SubstituteState.Create(substitutionContext);
            var receivedCalls = (IReceivedCalls) substituteState.FindInstanceFor(typeof(IReceivedCalls), null);
            var resultsSetter = (IResultSetter) substituteState.FindInstanceFor(typeof(IResultSetter), null);
            var callHandlerFactory = new CallHandlerFactory();
            return new CallRouter(substitutionContext, receivedCalls, resultsSetter, new RouteFactory(substituteState, callHandlerFactory));
        }
    }
}