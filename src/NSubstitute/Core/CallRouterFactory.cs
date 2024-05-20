using NSubstitute.Routing;

namespace NSubstitute.Core;

public class CallRouterFactory(IThreadLocalContext threadLocalContext, IRouteFactory routeFactory) : ICallRouterFactory
{
    public ICallRouter Create(ISubstituteState substituteState, bool canConfigureBaseCalls)
    {
        // Cache popular routes which are bound to the particular substitute state when it's possible.
        var factoryWithCachedRoutes = new RouteFactoryCacheWrapper(routeFactory);
        return new CallRouter(substituteState, threadLocalContext, factoryWithCachedRoutes, canConfigureBaseCalls);
    }
}