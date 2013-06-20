using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnBaseResultCallHandler : ICallHandler
    {
        public RouteAction Handle(ICall call)
        {
            return RouteAction.Return(call.CallBase());
        }
    }
}