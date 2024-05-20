using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class AddCallToQueryResultHandler(IThreadLocalContext threadContext) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        threadContext.RegisterInContextQuery(call);

        return RouteAction.Continue();
    }
}