using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class SetBaseForCallHandler : ICallHandler
    {
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallBaseSpecifications _callBaseSpecifications;
        private readonly MatchArgs _matchArgs;

        public SetBaseForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallBaseSpecifications callBaseSpecifications, MatchArgs matchArgs)
        {
            _callSpecificationFactory = callSpecificationFactory;
            _callBaseSpecifications = callBaseSpecifications;
            _matchArgs = matchArgs;
        }

        public RouteAction Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            _callBaseSpecifications.Add(callSpec);
            return RouteAction.Continue();
        }
    }
}