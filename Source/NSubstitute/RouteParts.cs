using System;
using System.Collections.Generic;
using NSubstitute.Routes.Handlers;

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
        private ICallActions _callActions;
        private ICallSpecificationFactory _callSpecificationFactory;
        private ICallHandler _doActionsCallHandler;

        public RouteParts(SubstituteState substituteState, object[] routeArguments)
        {
            _eventHandlerRegistry = substituteState.EventHandlerRegistry;
            _callSpecificationFactory = substituteState.CallSpecificationFactory;
            _callActions = substituteState.CallActions;
            _routeArguments = routeArguments;
            _recordingCallHandler = new RecordCallHandler(substituteState.CallStack, substituteState.CallResults);
            _checkReceivedCallHandler = new CheckReceivedCallHandler(substituteState.CallStack, substituteState.CallResults, _callSpecificationFactory);
            _propertySetterHandler = new PropertySetterHandler(substituteState.PropertyHelper, substituteState.ResultSetter);
            _eventSubscriptionHandler = new EventSubscriptionHandler(_eventHandlerRegistry);
            _returnDefaultHandler = new ReturnDefaultForCallHandler(substituteState.CallResults);
            _doActionsCallHandler = new DoActionsCallHandler(_callActions);
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
            if (partType == typeof(SetActionForCallHandler)) return new SetActionForCallHandler(_callSpecificationFactory, _callActions, (Action<object[]>)_routeArguments[0]);
            if (partType == typeof(DoActionsCallHandler)) return _doActionsCallHandler;
            throw new KeyNotFoundException("Could not find part for " + partType.FullName);
        }
    }
}