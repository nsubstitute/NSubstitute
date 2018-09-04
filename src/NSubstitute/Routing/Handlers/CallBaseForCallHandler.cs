using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Routing.Handlers
{
    public class CallBaseForCallHandler : ICallHandler
    {
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallBaseConfiguration _callBaseConfig;
        private readonly MatchArgs _matchArgs;

        public CallBaseForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallBaseConfiguration callBaseConfig, MatchArgs matchArgs)
        {
            _callSpecificationFactory = callSpecificationFactory ?? throw new ArgumentNullException(nameof(callSpecificationFactory));
            _callBaseConfig = callBaseConfig ?? throw new ArgumentNullException(nameof(callBaseConfig));
            _matchArgs = matchArgs ?? throw new ArgumentNullException(nameof(matchArgs));
        }

        public RouteAction Handle(ICall call)
        {
            if (!call.CanCallBase) throw CouldNotConfigureCallBaseException.ForSingleCall();

            var callSpec = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            _callBaseConfig.Include(callSpec);

            return RouteAction.Continue();
        }
    }
}