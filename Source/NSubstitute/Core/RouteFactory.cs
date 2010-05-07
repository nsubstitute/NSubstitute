using System;

namespace NSubstitute.Core
{
    public class RouteFactory : IRouteFactory
    {
        private readonly IRoutePartsFactory _routePartsFactory;

        public RouteFactory(IRoutePartsFactory routePartsFactory)
        {
            _routePartsFactory = routePartsFactory;
        }

        public IRoute Create<TRoute>(params object[] routeArguments) where TRoute : IRoute
        {
            var routeParts = _routePartsFactory.Create(routeArguments);
            var route = (IRoute) Activator.CreateInstance(typeof (TRoute), routeParts);
            return route;
        }
    }
}

