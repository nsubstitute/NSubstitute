using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class CheckCallReceivedRoute : IRoute, IRouteDefinition
    {
        private readonly IRouteParts _routeParts;

        public CheckCallReceivedRoute(IRouteParts routeParts)
        {
            _routeParts = routeParts;
        }

        public object Handle(ICall call)
        {
            _routeParts.GetPart<CheckReceivedCallHandler>().Handle(call);
            return _routeParts.GetPart<ReturnDefaultForReturnTypeHandler>().Handle(call);
        }

        public IEnumerable<Type> HandlerTypes
        {
            get { throw new NotImplementedException(); }
        }
    }
}