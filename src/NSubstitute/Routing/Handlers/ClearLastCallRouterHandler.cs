using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers;

/// <summary>
/// Clears last call router on SubstitutionContext for routes that do not require it.
/// </summary>
/// <remarks>
/// This is to help prevent static state bleeding over into future calls.
/// </remarks>
public class ClearLastCallRouterHandler(IThreadLocalContext threadContext) : ICallHandler
{
    public RouteAction Handle(ICall call)
    {
        threadContext.ClearLastCallRouter();

        return RouteAction.Continue();
    }
}