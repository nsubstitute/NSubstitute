using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public class RouteFactory : IRouteFactory
    {
        static RouteFactory()
        {
            ConstructRoute = x => new Route(x);
        }

        public static Func<IEnumerable<ICallHandler>, IRoute> ConstructRoute { get; private set; }

        private readonly ISubstituteState _substituteState;
        private readonly IRoutePartsFactory _routePartsFactory;

        public RouteFactory(ISubstituteState substituteState, IRoutePartsFactory routePartsFactory)
        {
            _substituteState = substituteState;
            _routePartsFactory = routePartsFactory;
        }

        public IRoute Create<TRouteDefinition>(params object[] routeArguments) where TRouteDefinition : IRouteDefinition
        {
            var routeParts = _routePartsFactory.Create(routeArguments);
            var routeDefinition = (IRouteDefinition) Activator.CreateInstance(typeof(TRouteDefinition));
            var handlers = routeDefinition.HandlerTypes.Select(x => routeParts.CreateCallHandler(x, _substituteState, routeArguments));
            return ConstructRoute(handlers);
        }
    }
}

