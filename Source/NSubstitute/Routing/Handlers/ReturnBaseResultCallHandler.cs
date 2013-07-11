using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnBaseResultCallHandler : ICallHandler
    {
        private readonly ICallBaseSpecifications _callBaseSpecifications;

        public ReturnBaseResultCallHandler(ICallBaseSpecifications callBaseSpecifications)
        {
            _callBaseSpecifications = callBaseSpecifications;
        }

        public RouteAction Handle(ICall call)
        {
            if (_callBaseSpecifications.DoesCallBase(call))
            {
                return RouteAction.Return(call.CallBase());
            }

            return RouteAction.Continue();
        }
    }
}