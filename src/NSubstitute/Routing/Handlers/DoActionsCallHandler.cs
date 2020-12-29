using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class DoActionsCallHandler :ICallHandler
    {
        private readonly ICallActions _callActions;

        public DoActionsCallHandler(ICallActions callActions)
        {
            _callActions = callActions;
        }

        public RouteAction Handle(ICall call)
        {
            _callActions.InvokeMatchingActions(call);

            return RouteAction.Continue();
        }
    }
}