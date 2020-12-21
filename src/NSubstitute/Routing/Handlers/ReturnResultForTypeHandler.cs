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
            if (_resultsForType.TryGetResult(call, out var result))
            {
                return RouteAction.Return(result);
            }

            return RouteAction.Continue();
        }
    }
}