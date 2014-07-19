using System;

using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class CustomCallHandlers : ICallHandler
    {
        private readonly ISubstitutionContext _substitutionContext;

        public CustomCallHandlers(ISubstitutionContext substitutionContext)
        {
            _substitutionContext = substitutionContext;
        }

        public RouteAction Handle(ICall call)
        {
            foreach (var customCallHandler in _substitutionContext.GetSubstituteContextFor(call.Target()).CustomCallHandlers)
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