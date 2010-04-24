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
        Func<ICall, object> _handleCall;

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
            RecordNextCall();
        }

        public object Route(ICall call)
        {
            _context.LastCallRouter(this);
            var result = _handleCall(call);
            RecordNextCall();
            return result;
        }

        private void RecordNextCall()
        {
            _handleCall = delegate(ICall call)
                              {
                                  _eventSubscriptionHandler.Handle(call);
                                  _propertySetterHandler.Handle(call);
                                  return _recordingCallHandler.Handle(call);
                              };
        }

        public void AssertNextCallHasBeenReceived()
        {
            _handleCall = _checkReceivedCallHandler.Handle;
        }

        public void RaiseEventFromNextCall(Func<ICall, object[]> argumentsToRaiseEventWith)
        {
            _handleCall = delegate(ICall call)
                              {
                                  _eventRaiser.Raise(call, argumentsToRaiseEventWith(call));
                                  return null;
                              }; 
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