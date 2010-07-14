using System;
using System.Collections.Generic;

namespace NSubstitute.Routing
{
    public interface IRouteDefinition
    {
        IEnumerable<Type> HandlerTypes { get; }
    }
}