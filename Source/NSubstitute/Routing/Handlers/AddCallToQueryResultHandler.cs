using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class AddCallToQueryResultHandler : ICallHandler
    {
        private readonly Query _query;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public AddCallToQueryResultHandler(Func<Query> query, ICallSpecificationFactory callSpecificationFactory)
        {
            _query = query();
            _callSpecificationFactory = callSpecificationFactory;
        }

        public RouteAction Handle(ICall call)
        {
            var target = call.Target();
            var callSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
            _query.Add(target, callSpec);
            return RouteAction.Continue();
        }
    }
}