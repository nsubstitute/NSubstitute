using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnResultForTypeHandler : ICallHandler
    {
        private readonly IResultsForType _resultsForType;

        public ReturnResultForTypeHandler(IResultsForType resultsForType)
        {
            _resultsForType = resultsForType;
        }

        public RouteAction Handle(ICall call)
        {
            return _resultsForType.TryGetResult(call, out var result)
                ? RouteAction.Return(result)
                : RouteAction.Continue();
        }
    }
}