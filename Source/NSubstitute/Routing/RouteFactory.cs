using System;

namespace NSubstitute.Routing
{
    public class RouteFactory : IRouteFactory
    {
        private readonly IRoutePartsFactory _routePartsFactory;

        public RouteFactory(IRoutePartsFactory routePartsFactory)
        {
            _routePartsFactory = routePartsFactory;
        }

        public IRoute Create<TRouteDefinition>(params object[] routeArguments) where TRouteDefinition : IRouteDefinition
        {
            var routeParts = _routePartsFactory.Create(routeArguments);
            var route = (IRoute) Activator.CreateInstance(typeof (TRouteDefinition), routeParts);
            return route;
        }
    }
}

