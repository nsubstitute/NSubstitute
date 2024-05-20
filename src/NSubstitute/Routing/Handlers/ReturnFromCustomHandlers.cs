using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

public class ReturnFromCustomHandlers(ICustomHandlers customHandlers) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        // Performance optimization, as enumerator retrieval allocates.
        if (customHandlers.Handlers.Count == 0)
        {
            return RouteAction.Continue();
        }

        foreach (var handler in customHandlers.Handlers)
        {
            var result = handler.Handle(call);
            if (result.HasReturnValue)
            {
                return result;
            }
        }

        return RouteAction.Continue();
    }
}
