using System;
using System.Linq;

namespace NSubstitute
{
    public class EventRaiser : IEventRaiser
    {
        readonly IEventHandlerRegistry _eventHandlerRegistry;

        public EventRaiser(IEventHandlerRegistry eventHandlerRegistry)
        {
            _eventHandlerRegistry = eventHandlerRegistry;
        }

        public void Raise(ICall call, object[] argumentsToRaiseEventWith)
        {
            var methodInfo = call.GetMethodInfo();
            var eventInfo = methodInfo.DeclaringType.GetEvents().First(
                x => (x.GetAddMethod() == methodInfo) || (x.GetRemoveMethod() == methodInfo));
            var handlers = _eventHandlerRegistry.GetHandlers(eventInfo.Name);
            foreach (Delegate handler in handlers)
            {
                handler.DynamicInvoke(argumentsToRaiseEventWith);
            }
        }
    }
}