using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;

namespace NSubstitute.Routing.Handlers
{
    public class EventSubscriptionHandler: ICallHandler
    {
        private readonly IEventHandlerRegistry _eventHandlerRegistry;

        public EventSubscriptionHandler(IEventHandlerRegistry eventHandlerRegistry)
        {
            _eventHandlerRegistry = eventHandlerRegistry;
        }

        public RouteAction Handle(ICall call)
        {
            if (CanBeSubscribeUnsubscribeCall(call))
            {
                If(call, IsEventSubscription, _eventHandlerRegistry.Add);
                If(call, IsEventUnsubscription, _eventHandlerRegistry.Remove);
            }

            return RouteAction.Continue();
        }

        private static bool CanBeSubscribeUnsubscribeCall(ICall call)
        {
            var methodInfo = call.GetMethodInfo();

            // It's safe to verify method prefix and signature as according to the ECMA-335 II.22.28:
            // 18. Any AddOn method for an event whose Name is xxx shall have the signature: void add_xxx (<DelegateType> handler) (§I.10.4) [CLS]
            // 19. Any RemoveOn method for an event whose Name is xxx shall have the signature: void remove_xxx(<DelegateType> handler) (§I.10.4) [CLS]
            // Notice, even though it's correct to check the SpecialName flag, we don't do that deliberately.
            // The reason is that some compilers (e.g. F#) might not emit this attribute and our library
            // misbehaves in those cases. We use slightly slower, but robust check.
            return methodInfo.ReturnType == typeof(void) &&
                   (methodInfo.Name.StartsWith("add_", StringComparison.Ordinal) ||
                    methodInfo.Name.StartsWith("remove_", StringComparison.Ordinal));
        }

        private static void If(ICall call, Func<ICall, Predicate<EventInfo>> meetsThisSpecification, Action<string, object> takeThisAction)
        {
            var matchingEvent = GetEvents(call, meetsThisSpecification).FirstOrDefault();
            if (matchingEvent != null)
            {
                // It's important to use original arguments, as it provides better performance.
                // It's safe to use original arguments here, as only by-ref arguments might be modified,
                // which should never happen for this case.
                takeThisAction(matchingEvent.Name, call.GetOriginalArguments()[0]!);
            }
        }

        private static Predicate<EventInfo> IsEventSubscription(ICall call) =>
            x => call.GetMethodInfo() == x.GetAddMethod();

        private static Predicate<EventInfo> IsEventUnsubscription(ICall call) =>
            x => call.GetMethodInfo() == x.GetRemoveMethod();

        private static IEnumerable<EventInfo> GetEvents(ICall call, Func<ICall, Predicate<EventInfo>> createPredicate)
        {
            var predicate = createPredicate(call);
            return call.GetMethodInfo().DeclaringType!.GetEvents().Where(x => predicate(x));
        }
    }
}