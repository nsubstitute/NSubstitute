using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        private readonly SequenceNumberGenerator _sequenceNumberGenerator;
        private readonly IRouteFactory _routeFactory;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public CallRouterFactory(SequenceNumberGenerator sequenceNumberGenerator,
            IRouteFactory routeFactory,
            ICallSpecificationFactory callSpecificationFactory)
        {
            _sequenceNumberGenerator = sequenceNumberGenerator;
            _routeFactory = routeFactory;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public ICallRouter Create(SubstituteConfig config, IThreadLocalContext threadContext, ISubstituteFactory substituteFactory)
        {
            var substituteState = new SubstituteState(config,
                _sequenceNumberGenerator,
                substituteFactory,
                _callSpecificationFactory);
            return new CallRouter(substituteState, threadContext, _routeFactory);
        }
    }
}