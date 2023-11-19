using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class AddCallToQueryResultHandler : ICallHandler
    {
        private readonly IThreadLocalContext _threadContext;

        public AddCallToQueryResultHandler(IThreadLocalContext threadContext)
        {
            _threadContext = threadContext;
        }

        public RouteAction Handle(ICall call)
        {
            _threadContext.RegisterInContextQuery(call);

            return RouteAction.Continue();
        }
    }
}