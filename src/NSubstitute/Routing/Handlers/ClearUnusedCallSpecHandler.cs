using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ClearUnusedCallSpecHandler : ICallHandler
    {
        private readonly IPendingSpecification _pendingSpecification;

        public ClearUnusedCallSpecHandler(IPendingSpecification pendingSpecification)
        {
            _pendingSpecification = pendingSpecification;
        }

        public RouteAction Handle(ICall call)
        {
            _pendingSpecification.Clear();

            return RouteAction.Continue();
        }
    }
}