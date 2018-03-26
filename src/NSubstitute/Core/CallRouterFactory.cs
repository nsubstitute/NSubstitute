using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        private readonly SequenceNumberGenerator _sequenceNumberGenerator;
        private readonly IRouteFactory _routeFactory;

        public CallRouterFactory(SequenceNumberGenerator sequenceNumberGenerator, IRouteFactory routeFactory)
        {
            _sequenceNumberGenerator = sequenceNumberGenerator;
            _routeFactory = routeFactory;
        }

        public ICallRouter Create(SubstituteConfig config, IThreadLocalContext threadContext, ISubstituteFactory substituteFactory)
        {
            var substituteState = new SubstituteState(config, _sequenceNumberGenerator, substituteFactory);
            return new CallRouter(substituteState, threadContext, _routeFactory);
        }
    }
}