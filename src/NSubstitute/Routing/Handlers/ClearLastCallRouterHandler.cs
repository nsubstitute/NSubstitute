using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    /// <summary>
    /// Clears last call router on SubstitutionContext for routes that do not require it.
    /// </summary>
    /// <remarks>
    /// This is to help prevent static state bleeding over into future calls.
    /// </remarks>
    public class ClearLastCallRouterHandler : ICallHandler
    {
        private readonly IThreadLocalContext _threadContext;

        public ClearLastCallRouterHandler(IThreadLocalContext threadContext)
        {
            _threadContext = threadContext;
        }

        public RouteAction Handle(ICall call)
        {
            _threadContext.ClearLastCallRouter();

            return RouteAction.Continue();
        }
    }
}