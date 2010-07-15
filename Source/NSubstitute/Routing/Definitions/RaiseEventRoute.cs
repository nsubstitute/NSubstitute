using System;
using System.Collections.Generic;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class RaiseEventRoute : IRouteDefinition
    {
        public IEnumerable<Type> HandlerTypes
        {
            get { return new[] {typeof (RaiseEventHandler), typeof (ReturnDefaultForReturnTypeHandler)}; }
        }
    }
}