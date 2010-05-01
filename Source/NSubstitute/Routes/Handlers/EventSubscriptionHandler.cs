using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Routes.Handlers
{
    public class EventSubscriptionHandler: ICallHandler
    {
        private readonly IEventHandlerRegistry _eventHandlerRegistry;

        public EventSubscriptionHandler(IEventHandlerRegistry eventHandlerRegistry)
        {
            _eventHandlerRegistry = eventHandlerRegistry;
        }

        public object Handle(ICall call)
        {
            If(call, IsEventSubscription, _eventHandlerRegistry.Add);
            If(call, IsEventUnsubscription, _eventHandlerRegistry.Remove);
            return null;
        }

        private void If(ICall call, Func<ICall, Predicate<EventInfo>> meetsThisSpecification, Action<string, object> takeThisAction)
        {
            var events = GetEvents(call, meetsThisSpecification);
            if (events.Any())
            {
                takeThisAction(events.First().Name, call.GetArguments()[0]);
            }            
        }

        private Predicate<EventInfo> IsEventSubscription(ICall call)
        {
            return x => call.GetMethodInfo() == x.GetAddMethod();
        }

        private Predicate<EventInfo> IsEventUnsubscription(ICall call)
        {
            return x => call.GetMethodInfo() == x.GetRemoveMethod();
        }

        private IEnumerable<EventInfo> GetEvents(ICall call, Func<ICall, Predicate<EventInfo>> createPredicate)
        {
            var predicate = createPredicate(call);
            return call.GetMethodInfo().DeclaringType.GetEvents().Where(x => predicate(x));
        }
    }
}