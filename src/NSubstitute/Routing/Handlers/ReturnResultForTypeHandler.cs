using System;
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
            return _resultsForType.HasResultFor(call)
                       ? RouteAction.Return(_resultsForType.GetResult(call))
                       : RouteAction.Continue();
        }
    }
}