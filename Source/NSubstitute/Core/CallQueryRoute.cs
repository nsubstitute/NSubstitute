using System;
using System.Collections.Generic;
using NSubstitute.Routing;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Core
{
    public class CallQueryRoute : IRouteDefinition
    {
        public IEnumerable<Type> HandlerTypes
        {
            get
            {
                return new[]
                           { 
                               typeof (ClearUnusedCallSpecHandler),
                               typeof (AddCallToQueryResultHandler), 
                               typeof (ReturnConfiguredResultHandler), 
                               typeof (ReturnAutoValueForThisAndSubsequentCallsHandler),
                               typeof (ReturnOriginalResultCallHandler),
                               typeof (ReturnDefaultForReturnTypeHandler)
                           };
            }
        }
    }
}