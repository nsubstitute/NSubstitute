using System;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class CallRouterFactory : ICallRouterFactory
    {
        private readonly IThreadLocalContext _threadLocalContext;
        private readonly IRouteFactory _routeFactory;

        public CallRouterFactory(IThreadLocalContext threadLocalContext, IRouteFactory routeFactory)
        {
            _threadLocalContext = threadLocalContext;
            _routeFactory = routeFactory;
        }

        public ICallRouter Create(ISubstituteState substituteState, bool canConfigureBaseCalls)
        {
            // Cache popular routes which are bound to the particular substitute state when it's possible.
            var factoryWithCachedRoutes = new RouteFactoryCacheWrapper(_routeFactory);
            return new CallRouter(substituteState, _threadLocalContext, factoryWithCachedRoutes, canConfigureBaseCalls);
        }
    }
}