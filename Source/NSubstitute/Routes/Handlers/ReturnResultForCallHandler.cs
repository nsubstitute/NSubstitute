using NSubstitute.Core;

namespace NSubstitute.Routes.Handlers
{
    public class ReturnResultForCallHandler : ICallHandler
    {
        private readonly ICallResults _callResults;
        private readonly IDefaultForType _defaultForType;

        public ReturnResultForCallHandler(ICallResults callResults, IDefaultForType defaultForType)
        {
            _callResults = callResults;
            _defaultForType = defaultForType;
        }

        public object Handle(ICall call)
        {
            if (_callResults.HasResultFor(call))
            {
                return _callResults.GetResult(call);
            }
            return _defaultForType.GetDefaultFor(call.GetReturnType());
        }
    }
}