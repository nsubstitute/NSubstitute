using System.Linq;

namespace NSubstitute
{
    public class RecordCallHandler : ICallHandler
    {
        private readonly ICallStack _callStack;
        private readonly ICallResults _configuredResults;

        public RecordCallHandler(ICallStack callStack, ICallResults configuredResults)
        {
            _callStack = callStack;
            _configuredResults = configuredResults;
        }

        public object Handle(ICall call)
        {
            _callStack.Push(call);
            return _configuredResults.GetResult(call);
        }
    }
}