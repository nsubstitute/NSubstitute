using NSubstitute.Routes.Handlers;

namespace NSubstitute.Routes
{
    public class CheckCallReceivedRoute : IRoute
    {
        private readonly IRouteParts _routeParts;

        public CheckCallReceivedRoute(IRouteParts routeParts)
        {
            _routeParts = routeParts;
        }

        public object Handle(ICall call)
        {
            _routeParts.GetPart<CheckReceivedCallHandler>().Handle(call);
            return _routeParts.GetPart<ReturnDefaultForCallHandler>().Handle(call);
        }
    }
}