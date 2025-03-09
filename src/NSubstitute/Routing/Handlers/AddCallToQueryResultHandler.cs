using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

internal sealed class AddCallToQueryResultHandler(IThreadLocalContext threadContext) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        threadContext.RegisterInContextQuery(call);

        return RouteAction.Continue();
    }
}