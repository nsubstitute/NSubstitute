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
        private readonly ICallHandlerFactory _callHandlerFactory;

        public RouteFactory(ISubstituteState substituteState, ICallHandlerFactory callHandlerFactory)
        {
            _substituteState = substituteState;
            _callHandlerFactory = callHandlerFactory;
        }

        public IRoute Create<TRouteDefinition>(params object[] routeArguments) where TRouteDefinition : IRouteDefinition
        {
            var routeDefinition = (IRouteDefinition) Activator.CreateInstance(typeof(TRouteDefinition));
            var handlers = routeDefinition.HandlerTypes.Select(x => _callHandlerFactory.CreateCallHandler(x, _substituteState, routeArguments));
            return ConstructRoute(handlers);
        }
    }
}

