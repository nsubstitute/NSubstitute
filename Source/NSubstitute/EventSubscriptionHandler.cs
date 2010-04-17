using System;

namespace NSubstitute
{
    public class EventSubscriptionHandler: ICallHandler
    {
        private const string MethodNamePrefixForAdd = "add_";
        private const string MethodNamePrefixForRemove = "remove_";
        private readonly IEventHandlerRegistry _eventHandlerRegistry;

        public EventSubscriptionHandler(IEventHandlerRegistry eventHandlerRegistry)
        {
            _eventHandlerRegistry = eventHandlerRegistry;
        }

        public object Handle(ICall call)
        {
            var methodInfo = call.GetMethodInfo();
            if (IsEventSubscription(call))
            {
                var eventName = methodInfo.Name.Substring(MethodNamePrefixForAdd.Length);
                _eventHandlerRegistry.Add(eventName, call.GetArguments()[0]);
            }
            if (IsEventUnsubscription(call))
            {
                var eventName = methodInfo.Name.Substring(MethodNamePrefixForRemove.Length);
                _eventHandlerRegistry.Remove(eventName, call.GetArguments()[0]);            
            }
            return null;
        }

        private bool IsEventUnsubscription(ICall call)
        {
            return call.GetMethodInfo().Name.StartsWith(MethodNamePrefixForRemove);
        }

        private bool IsEventSubscription(ICall call)
        {
            return call.GetMethodInfo().Name.StartsWith(MethodNamePrefixForAdd);
        }
    }
}