using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Routing.Definitions
{
    public class RecordReplayRoute : IRoute, IRouteDefinition
    {
        private readonly IRouteParts _routeParts;

        public RecordReplayRoute(IRouteParts routeParts)
        {
            _routeParts = routeParts;
        }

        public object Handle(ICall call)
        {
            _routeParts.GetPart<EventSubscriptionHandler>().Handle(call);
            _routeParts.GetPart<PropertySetterHandler>().Handle(call);
            _routeParts.GetPart<DoActionsCallHandler>().Handle(call);
            _routeParts.GetPart<RecordCallHandler>().Handle(call);
            return _routeParts.GetPart<ReturnConfiguredResultHandler>().Handle(call);
        }

        public IEnumerable<Type> HandlerTypes
        {
            get
            {
                return new[]
                           {
                               typeof (EventSubscriptionHandler), typeof (PropertySetterHandler),
                               typeof (DoActionsCallHandler), typeof (RecordCallHandler),
                               typeof (ReturnConfiguredResultHandler)
                           };
            }
        }
    }
}