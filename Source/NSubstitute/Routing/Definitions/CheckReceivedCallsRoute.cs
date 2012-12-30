using System;
using System.Collections.Generic;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class CheckReceivedCallsRoute : IRouteDefinition
    {
        public IEnumerable<Type> HandlerTypes
        {
            get { return new[] {
                    typeof (ClearLastCallRouterHandler),
                    typeof (ClearUnusedCallSpecHandler),
                    typeof (CheckReceivedCallsHandler), 
                    typeof (ReturnDefaultForReturnTypeHandler)}; }
        }
    }
}