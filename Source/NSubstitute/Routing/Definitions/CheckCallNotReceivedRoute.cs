using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class CheckCallNotReceivedRoute : IRoute, IRouteDefinition
    {
        private readonly IRouteParts _routeParts;

        public CheckCallNotReceivedRoute(IRouteParts routeParts)
        {
            _routeParts = routeParts;
        }

        public object Handle(ICall call)
        {
            _routeParts.GetPart<CheckDidNotReceiveCallHandler>().Handle(call);
            return _routeParts.GetPart<ReturnDefaultForReturnTypeHandler>().Handle(call);
        }

        public IEnumerable<Type> HandlerTypes
        {
            get { throw new NotImplementedException(); }
        }
    }
}