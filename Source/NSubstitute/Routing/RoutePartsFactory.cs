using NSubstitute.Core;

namespace NSubstitute.Routing
{
    public class RoutePartsFactory : IRoutePartsFactory
    {
        private SubstituteState _substituteState;

        public RoutePartsFactory(SubstituteState substituteState)
        {
            _substituteState = substituteState;
        }

        public IRouteParts Create(params object[] routeArguments)
        {
            return new RouteParts(_substituteState, routeArguments);
        }
    }
}