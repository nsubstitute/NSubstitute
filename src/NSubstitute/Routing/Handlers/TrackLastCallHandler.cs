using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class TrackLastCallHandler : ICallHandler
    {
        private readonly IPendingSpecification _pendingSpecification;

        public TrackLastCallHandler(IPendingSpecification pendingSpecification)
        {
            _pendingSpecification = pendingSpecification;
        }

        public RouteAction Handle(ICall call)
        {
            _pendingSpecification.SetLastCall(call);

            return RouteAction.Continue();
        }
    }
}