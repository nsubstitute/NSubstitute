using System;
using System.Collections.Generic;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class CheckCallReceivedRoute : IRouteDefinition
    {
        public IEnumerable<Type> HandlerTypes
        {
            get { return new[] {typeof (CheckReceivedCallHandler), typeof (ReturnDefaultForReturnTypeHandler)}; }
        }
    }
}