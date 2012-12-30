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
        private readonly ISubstitutionContext _context;

        public ClearLastCallRouterHandler(ISubstitutionContext context)
        {
            _context = context;
        }

        public RouteAction Handle(ICall call)
        {
            _context.ClearLastCallRouter();
            return RouteAction.Continue();
        }
    }
}