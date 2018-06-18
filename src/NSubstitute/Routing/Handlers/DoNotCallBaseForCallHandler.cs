using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class DoNotCallBaseForCallHandler :ICallHandler
    {
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallBaseConfiguration _callBaseConfig;
        private readonly MatchArgs _matchArgs;

        public DoNotCallBaseForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallBaseConfiguration callBaseConfig, MatchArgs matchArgs)
        {
            _callSpecificationFactory = callSpecificationFactory ?? throw new ArgumentNullException(nameof(callSpecificationFactory));
            _callBaseConfig = callBaseConfig ?? throw new ArgumentNullException(nameof(callBaseConfig));
            _matchArgs = matchArgs ?? throw new ArgumentNullException(nameof(matchArgs));
        }

        public RouteAction Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            _callBaseConfig.Exclude(callSpec);

            return RouteAction.Continue();
        }
    }
}