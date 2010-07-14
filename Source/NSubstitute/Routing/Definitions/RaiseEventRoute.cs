using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class RaiseEventRoute : IRoute, IRouteDefinition
    {
        private readonly IRouteParts _routeParts;

        public RaiseEventRoute(IRouteParts routeParts)
        {
            _routeParts = routeParts;
        }

        public object Handle(ICall call)
        {
            _routeParts.GetPart<RaiseEventHandler>().Handle(call);
            return _routeParts.GetPart<ReturnDefaultForReturnTypeHandler>().Handle(call);            
        }

        public IEnumerable<Type> HandlerTypes
        {
            get { throw new NotImplementedException(); }
        }
    }
}