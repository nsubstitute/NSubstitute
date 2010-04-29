using System;

namespace NSubstitute
{
    public class CallRouter : ICallRouter
    {
        readonly ISubstitutionContext _context;
        readonly ICallHandler _recordingCallHandler;
        readonly ICallHandler _checkReceivedCallHandler;
        readonly ICallHandler _propertySetterHandler;
        readonly ICallHandler _eventSubscriptionHandler;
        readonly IResultSetter _resultSetter;
        readonly IEventRaiser _eventRaiser;
        IRoute _currentRoute;

        public CallRouter(ISubstitutionContext context, ICallHandler recordingCallHandler, 
                            ICallHandler propertySetterHandler, ICallHandler eventSubscriptionHandler, 
                            ICallHandler checkReceivedCallHandler, IResultSetter resultSetter, 
                            IEventRaiser eventRaiser)
        {
            _context = context;
            _recordingCallHandler = recordingCallHandler;
            _checkReceivedCallHandler = checkReceivedCallHandler;
            _propertySetterHandler = propertySetterHandler;
            _eventSubscriptionHandler = eventSubscriptionHandler;
            _resultSetter = resultSetter;
            _eventRaiser = eventRaiser;
            RecordAndReplayOnNextCall();
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            var result = _currentRoute.Handle(call);
            RecordAndReplayOnNextCall();
            return result;
        }

        private void RecordAndReplayOnNextCall()
        {
            _currentRoute = new RecordReplayRoute(_eventSubscriptionHandler, _propertySetterHandler, _recordingCallHandler);
        }

        public void AssertNextCallHasBeenReceived()
        {
            _currentRoute = new CheckCallReceivedRoute(_checkReceivedCallHandler);
        }

        public void RaiseEventFromNextCall(Func<ICall, object[]> argumentsToRaiseEventWith)
        {
            _currentRoute = new RaiseEventRoute(_eventRaiser, argumentsToRaiseEventWith);
        }

        public void AddCallbackForNextCall(Action<object[]> callbackWithArguments)
        {
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            _resultSetter.SetResultForLastCall(valueToReturn);
        }
    }
}