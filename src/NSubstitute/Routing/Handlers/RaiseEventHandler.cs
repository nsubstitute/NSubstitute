using System;
using System.Linq;
using System.Reflection;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Routing.Handlers
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

        public RouteAction Handle(ICall call)
        {
            var methodInfo = call.GetMethodInfo();
            var eventInfo = methodInfo.DeclaringType.GetEvents().FirstOrDefault(
                x => (x.GetAddMethod() == methodInfo) || (x.GetRemoveMethod() == methodInfo));
            if (eventInfo == null) throw new CouldNotRaiseEventException();
            var handlers = _eventHandlerRegistry.GetHandlers(eventInfo.Name);
            var eventArguments = _getEventArguments(call);
            foreach (Delegate handler in handlers)
            {
                try
                {
                    handler.DynamicInvoke(eventArguments);
                }
                catch (TargetInvocationException e)
                {
                    PreserveStackTrace(e.InnerException);

                    throw e.InnerException;
                }
            }
            return RouteAction.Continue();
        }

        private void PreserveStackTrace(Exception exception)
        {
#if NET35 || NET4 || NET45
            var context = new StreamingContext(StreamingContextStates.CrossAppDomain);
            var serializationInfo = new SerializationInfo(typeof(Exception), new FormatterConverter());
            var constructor = typeof(Exception).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);

            exception.GetObjectData(serializationInfo, context);
            constructor.Invoke(exception, new object[] { serializationInfo, context });
#endif
        }
    }
}