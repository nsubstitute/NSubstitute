namespace NSubstitute.Routes.Handlers
{
    public class DoActionsCallHandler :ICallHandler
    {
        private readonly ICallActions _callActions;

        public DoActionsCallHandler(ICallActions callActions)
        {
            _callActions = callActions;
        }

        public object Handle(ICall call)
        {
            var actions = _callActions.MatchingActions(call);
            foreach (var action in actions)
            {
                action(call.GetArguments());
            }
            return null;
        }
    }
}