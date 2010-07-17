using System;
using System.Collections.Generic;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class RecordReplayRoute : IRouteDefinition
    {
        public IEnumerable<Type> HandlerTypes
        {
            get
            {
                return new[]
                           {
                               typeof (EventSubscriptionHandler), typeof (PropertySetterHandler),
                               typeof (DoActionsCallHandler), typeof (RecordCallHandler),
                               typeof (ReturnConfiguredResultHandler), typeof(ReturnDefaultForReturnTypeHandler)
                           };
            }
        }
    }
}