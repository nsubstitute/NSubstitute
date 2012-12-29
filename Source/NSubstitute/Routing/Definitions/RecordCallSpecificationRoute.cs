using System;
using System.Collections.Generic;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class RecordCallSpecificationRoute : IRouteDefinition
    {
        public IEnumerable<Type> HandlerTypes
        {
            get
            {
                return new[] {
                               typeof (RecordCallSpecificationHandler),
                               typeof (PropertySetterHandler),
                               typeof (ReturnConfiguredResultHandler),
                               typeof (ReturnAutoValueForThisAndSubsequentCallsHandler),
                               typeof (ReturnDefaultForReturnTypeHandler)
                           };
            }
        }
    }
}