using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public class RouteParts : IRouteParts
    {
        private IEventHandlerRegistry _eventHandlerRegistry;
        private readonly object[] _routeArguments;
        private RecordCallHandler _recordingCallHandler;
        private CheckReceivedCallHandler _checkReceivedCallHandler;
        private PropertySetterHandler _propertySetterHandler;
        private EventSubscriptionHandler _eventSubscriptionHandler;
        private ReturnDefaultForCallHandler _returnDefaultHandler;

        public RouteParts(SubstituteState substituteState, object[] routeArguments)
        {
            _eventHandlerRegistry = substituteState.EventHandlerRegistry;            
            _routeArguments = routeArguments;
            _recordingCallHandler = new RecordCallHandler(substituteState.CallStack, substituteState.CallResults);
            _checkReceivedCallHandler = new CheckReceivedCallHandler(substituteState.CallStack, substituteState.CallResults, substituteState.CallSpecificationFactory);
            _propertySetterHandler = new PropertySetterHandler(substituteState.ReflectionHelper, substituteState.ResultSetter);
            _eventSubscriptionHandler = new EventSubscriptionHandler(_eventHandlerRegistry);
            _returnDefaultHandler = new ReturnDefaultForCallHandler(substituteState.CallResults);
        }

        public ICallHandler GetPart<TPart>() where TPart : ICallHandler
        {
            var partType = typeof(TPart);
            if (partType == typeof(RecordCallHandler)) return _recordingCallHandler;
            if (partType == typeof(CheckReceivedCallHandler)) return _checkReceivedCallHandler;
            if (partType == typeof(PropertySetterHandler)) return _propertySetterHandler;
            if (partType == typeof(EventSubscriptionHandler)) return _eventSubscriptionHandler;
            if (partType == typeof(RaiseEventHandler)) return new RaiseEventHandler(_eventHandlerRegistry, (Func<ICall, object[]>)_routeArguments[0]);
            if (partType == typeof(ReturnDefaultForCallHandler)) return _returnDefaultHandler;
            throw new KeyNotFoundException("Could not find part for " + partType.FullName);
        }
    }
}