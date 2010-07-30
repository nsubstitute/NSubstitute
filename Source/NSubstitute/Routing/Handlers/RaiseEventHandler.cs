using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NSubstitute.Core;

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
            var eventInfo = methodInfo.DeclaringType.GetEvents().First(
                x => (x.GetAddMethod() == methodInfo) || (x.GetRemoveMethod() == methodInfo));
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
            var context = new StreamingContext(StreamingContextStates.CrossAppDomain);
            var objectManager = new ObjectManager(null, context);
            var serializationInfo = new SerializationInfo(exception.GetType(), new FormatterConverter());

            exception.GetObjectData(serializationInfo, context);
            objectManager.RegisterObject(exception, 1, serializationInfo); 
            objectManager.DoFixups();
        }
    }
}