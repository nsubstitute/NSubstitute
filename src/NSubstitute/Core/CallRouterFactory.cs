using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        private readonly SequenceNumberGenerator _sequenceNumberGenerator;
        private readonly IRouteFactory _routeFactory;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallInfoFactory _callInfoFactory;

        public CallRouterFactory(SequenceNumberGenerator sequenceNumberGenerator,
            IRouteFactory routeFactory,
            ICallSpecificationFactory callSpecificationFactory,
            ICallInfoFactory callInfoFactory)
        {
            _sequenceNumberGenerator = sequenceNumberGenerator;
            _routeFactory = routeFactory;
            _callSpecificationFactory = callSpecificationFactory;
            _callInfoFactory = callInfoFactory;
        }

        public ICallRouter Create(SubstituteConfig config, IThreadLocalContext threadContext, ISubstituteFactory substituteFactory)
        {
            var substituteState = new SubstituteState(config,
                _sequenceNumberGenerator,
                substituteFactory,
                _callSpecificationFactory,
                _callInfoFactory);

            return new CallRouter(substituteState, threadContext, _routeFactory);
        }
    }
}