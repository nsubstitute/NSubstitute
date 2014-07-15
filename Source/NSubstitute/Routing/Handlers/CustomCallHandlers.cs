using System;

using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class CustomCallHandlers : ICallHandler
    {
        private readonly Func<ICall, RouteAction>[] _customCallHandlers;

        public CustomCallHandlers(Func<ICall, RouteAction>[] customCallHandlers)
        {
            _customCallHandlers = customCallHandlers;
        }

        public RouteAction Handle(ICall call)
        {
            foreach (var customCallHandler in _customCallHandlers)
            {
                var routeAction = customCallHandler(call);
                if (routeAction.HasReturnValue)
                {
                    return routeAction;
                }
            }

            return RouteAction.Continue();
        }
    }
}