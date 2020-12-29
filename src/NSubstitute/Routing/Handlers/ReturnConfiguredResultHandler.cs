using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnConfiguredResultHandler : ICallHandler
    {
        private readonly ICallResults _callResults;

        public ReturnConfiguredResultHandler(ICallResults callResults)
        {
            _callResults = callResults;
        }

        public RouteAction Handle(ICall call)
        {
            if (_callResults.TryGetResult(call, out var configuredResult))
            {
                return RouteAction.Return(configuredResult);
            }

            return RouteAction.Continue();
        }
    }
}