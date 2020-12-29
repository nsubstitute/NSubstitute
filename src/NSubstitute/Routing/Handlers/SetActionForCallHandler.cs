using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class SetActionForCallHandler :ICallHandler
    {
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallActions _callActions;
        private readonly Action<CallInfo> _action;
        private readonly MatchArgs _matchArgs;

        public SetActionForCallHandler(ICallSpecificationFactory callSpecificationFactory, ICallActions callActions, Action<CallInfo> action, MatchArgs matchArgs)
        {
            _callSpecificationFactory = callSpecificationFactory;
            _callActions = callActions;
            _action = action;
            _matchArgs = matchArgs;
        }

        public RouteAction Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, _matchArgs);
            _callActions.Add(callSpec, _action);

            return RouteAction.Continue();
        }
    }
}