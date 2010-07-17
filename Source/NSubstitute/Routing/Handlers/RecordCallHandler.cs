using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class RecordCallHandler : ICallHandler
    {
        private readonly ICallStack _callStack;

        public RecordCallHandler(ICallStack callStack)
        {
            _callStack = callStack;
        }

        public RouteAction Handle(ICall call)
        {
            _callStack.Push(call);
            return RouteAction.Continue();
        }
    }
}