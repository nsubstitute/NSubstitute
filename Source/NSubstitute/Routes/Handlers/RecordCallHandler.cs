using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class RecordCallHandler : ICallHandler
    {
        private readonly ICallStack _callStack;

        public RecordCallHandler(ICallStack callStack)
        {
            _callStack = callStack;
        }

        public object Handle(ICall call)
        {
            _callStack.Push(call);
            return null;
        }
    }
}