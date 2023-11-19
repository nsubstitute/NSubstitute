using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnDefaultForReturnTypeHandler : ICallHandler
    {
        private readonly IDefaultForType _defaultForType;

        public ReturnDefaultForReturnTypeHandler(IDefaultForType defaultForType)
        {
            _defaultForType = defaultForType;
        }

        public RouteAction Handle(ICall call)
        {
            var returnValue = _defaultForType.GetDefaultFor(call.GetMethodInfo().ReturnType);
            return RouteAction.Return(returnValue);
        }
    }
}