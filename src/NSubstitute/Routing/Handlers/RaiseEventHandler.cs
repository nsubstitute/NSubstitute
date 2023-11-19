using System;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Routing.Handlers
{
    public class RaiseEventHandler : ICallHandler
    {
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly Func<ICall, object?[]> _getEventArguments;

        public RaiseEventHandler(IEventHandlerRegistry eventHandlerRegistry, Func<ICall, object?[]> getEventArguments)
        {
            _eventHandlerRegistry = eventHandlerRegistry;
            _getEventArguments = getEventArguments;
        }

        public RouteAction Handle(ICall call)
        {
            var methodInfo = call.GetMethodInfo();
            var eventInfo = FindEventInfo(methodInfo);
            if (eventInfo == null)
            {
                throw new CouldNotRaiseEventException();
            }

            object?[] eventArguments = _getEventArguments(call);
            var handlers = _eventHandlerRegistry.GetHandlers(eventInfo.Name);
            foreach (Delegate handler in handlers)
            {
                if (handler == null)
                {
                    continue;
                }

                try
                {
                    handler.DynamicInvoke(eventArguments);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException!;
                }
            }

            return RouteAction.Continue();

            static EventInfo? FindEventInfo(MethodInfo mi)
            {
                return mi.DeclaringType!.GetEvents().FirstOrDefault(
                    e => e.GetAddMethod() == mi || e.GetRemoveMethod() == mi);
            }
        }
    }
}