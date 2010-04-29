using System;
using System.Linq;

namespace NSubstitute
{
    public class RaiseEventHandler : ICallHandler
    {
        readonly IEventHandlerRegistry _eventHandlerRegistry;
        readonly Func<ICall, object[]> _getEventArguments;

        public RaiseEventHandler(IEventHandlerRegistry eventHandlerRegistry, Func<ICall, object[]> getEventArguments)
        {
            _eventHandlerRegistry = eventHandlerRegistry;
            _getEventArguments = getEventArguments;
        }

        public object Handle(ICall call)
        {
            var methodInfo = call.GetMethodInfo();
            var eventInfo = methodInfo.DeclaringType.GetEvents().First(
                x => (x.GetAddMethod() == methodInfo) || (x.GetRemoveMethod() == methodInfo));
            var handlers = _eventHandlerRegistry.GetHandlers(eventInfo.Name);
            var eventArguments = _getEventArguments(call);
            foreach (Delegate handler in handlers)
            {
                handler.DynamicInvoke(eventArguments);
            }
            return null;
        }
    }
}