using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class ReturnOriginalResultCallHandler : ICallHandler
    {
        public RouteAction Handle(ICall call)
        {
            // We want the proxies to respond to the base object methods (e.g. Equals, GetHashCode)
            if (call.GetMethodInfo().GetBaseDefinition().DeclaringType == typeof(object))
                return RouteAction.Return(call.CallOriginalMethod());

            return RouteAction.Continue();
        }
    }
}
