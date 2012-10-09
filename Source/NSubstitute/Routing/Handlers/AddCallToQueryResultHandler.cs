using System;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class AddCallToQueryResultHandler : ICallHandler
    {
        private readonly ISubstitutionContext _context;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public AddCallToQueryResultHandler(ISubstitutionContext context, ICallSpecificationFactory callSpecificationFactory)
        {
            _context = context;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public RouteAction Handle(ICall call)
        {
            var target = call.Target();
            var callSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
            _context.AddToQuery(target, callSpec);
            return RouteAction.Continue();
        }
    }
}