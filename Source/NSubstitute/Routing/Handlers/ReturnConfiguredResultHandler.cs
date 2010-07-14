using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnConfiguredResultHandler : ICallHandler
    {
        private readonly ICallResults _callResults;
        private readonly IDefaultForType _defaultForType;

        public ReturnConfiguredResultHandler(ICallResults callResults, IDefaultForType defaultForType)
        {
            _callResults = callResults;
            _defaultForType = defaultForType;
        }

        public object Handle(ICall call)
        {
            if (!_callResults.HasResultFor(call))
            {
                return _defaultForType.GetDefaultFor(call.GetReturnType());
            }
            return _callResults.GetResult(call);
        }
    }
}