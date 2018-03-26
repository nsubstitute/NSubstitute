using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class AddCallToQueryResultHandler : ICallHandler
    {
        private readonly IThreadLocalContext _threadContext;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public AddCallToQueryResultHandler(IThreadLocalContext threadContext, ICallSpecificationFactory callSpecificationFactory)
        {
            _threadContext = threadContext;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public RouteAction Handle(ICall call)
        {
            var target = call.Target();
            var callSpec = _callSpecificationFactory.CreateFrom(call, MatchArgs.AsSpecifiedInCall);
            _threadContext.AddToQuery(target, callSpec);
            return RouteAction.Continue();
        }
    }
}