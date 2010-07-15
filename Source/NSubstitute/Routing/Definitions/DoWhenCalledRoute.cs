using System;
using System.Collections.Generic;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class DoWhenCalledRoute : IRouteDefinition
    {
        public IEnumerable<Type> HandlerTypes
        {
            get { return new[] {typeof (SetActionForCallHandler), typeof (ReturnDefaultForReturnTypeHandler)}; }
        }
    }
}