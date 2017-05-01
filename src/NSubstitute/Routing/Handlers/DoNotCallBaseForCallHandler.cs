using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class DoNotCallBaseForCallHandler :ICallHandler
    {
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallBaseExclusions _exclusions;
        private readonly MatchArgs _matchArgs;

        public DoNotCallBaseForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallBaseExclusions exclusions, MatchArgs matchArgs)
        {
            _callSpecificationFactory = callSpecificationFactory;
            _exclusions = exclusions;
            _matchArgs = matchArgs;
        }

        public RouteAction Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            _exclusions.Exclude(callSpec);
            return RouteAction.Continue();
        }
    }
}