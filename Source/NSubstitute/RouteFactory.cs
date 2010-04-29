using System;
using System.Collections.Generic;

namespace NSubstitute
{
    public class RouteFactory : IRouteFactory
    {
        private ICallHandler _eventSubscriptionHandler;
        private ICallHandler _propertySetterHandler;
        private ICallHandler _recordingCallHandler;
        private ICallHandler _checkReceivedCallHandler;
        private IEventRaiser _eventRaiser;

        public RouteFactory(ICallHandler eventSubscriptionHandler, ICallHandler propertySetterHandler, ICallHandler recordingCallHandler, ICallHandler checkReceivedCallHandler, IEventRaiser eventRaiser)
        {
            _eventSubscriptionHandler = eventSubscriptionHandler;
            _propertySetterHandler = propertySetterHandler;
            _recordingCallHandler = recordingCallHandler;
            _checkReceivedCallHandler = checkReceivedCallHandler;
            _eventRaiser = eventRaiser;
        }

        public IRoute Create<TRoute>(params object[] routeArguments) where TRoute : IRoute
        {
            var routeType = typeof (TRoute);
            if (routeType == typeof(RecordReplayRoute)) return RecordAndReplayOnNextCall();
            if (routeType == typeof(CheckCallReceivedRoute)) return AssertNextCallHasBeenReceived();
            if (routeType == typeof(RaiseEventRoute)) return RaiseEventFromNextCall((Func<ICall, object[]>) routeArguments[0]);
            throw new KeyNotFoundException("Could not find route type for " + routeType.FullName);
        }

        private IRoute RecordAndReplayOnNextCall()
        {
            return new RecordReplayRoute(_eventSubscriptionHandler, _propertySetterHandler, _recordingCallHandler);
        }

        public IRoute AssertNextCallHasBeenReceived()
        {
            return new CheckCallReceivedRoute(_checkReceivedCallHandler);
        }

        public IRoute RaiseEventFromNextCall(Func<ICall, object[]> argumentsToRaiseEventWith)
        {
            return new RaiseEventRoute(_eventRaiser, argumentsToRaiseEventWith);
        }
    }
}