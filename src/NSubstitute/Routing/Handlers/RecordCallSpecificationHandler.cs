using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class RecordCallSpecificationHandler : ICallHandler
    {
        private readonly IPendingSpecification _pendingCallSpecification;
        private readonly ICallSpecificationFactory _callSpecificationFactory;
        private readonly ICallActions _callActions;

        public RecordCallSpecificationHandler(IPendingSpecification pendingCallSpecification, ICallSpecificationFactory callSpecificationFactory, ICallActions callActions)
        {
            _pendingCallSpecification = pendingCallSpecification;
            _callSpecificationFactory = callSpecificationFactory;
            _callActions = callActions;
        }

        public RouteAction Handle(ICall call)
        {
            var callSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
            _pendingCallSpecification.SetCallSpecification(callSpec);

            // Performance optimization - don't register call actions if current argument matchers
            // don't have any callbacks.
            if (call.GetArgumentSpecifications().Any(x => x.HasAction))
            {
                _callActions.Add(callSpec);
            }

            return RouteAction.Continue();
        }
    }
}