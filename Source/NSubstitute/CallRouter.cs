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
        Func<ICall, object> _handleCall;

        public CallRouter(ISubstitutionContext context, ICallHandler recordingCallHandler, ICallHandler propertySetterHandler, ICallHandler eventSubscriptionHandler, ICallHandler checkReceivedCallHandler, IResultSetter resultSetter)
        {
            _context = context;
            _recordingCallHandler = recordingCallHandler;
            _checkReceivedCallHandler = checkReceivedCallHandler;
            _propertySetterHandler = propertySetterHandler;
            _eventSubscriptionHandler = eventSubscriptionHandler;
            _resultSetter = resultSetter;
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

        public void RaiseEventFromNextCall(params object[] argumentsToRaiseEventWith)
        {            
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            _resultSetter.SetResultForLastCall(valueToReturn);
        }
    }
}