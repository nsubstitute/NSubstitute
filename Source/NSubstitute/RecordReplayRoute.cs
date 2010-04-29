namespace NSubstitute
{
    public class RecordReplayRoute : IRoute
    {
        private readonly ICallHandler _eventSubscriptionHandler;
        private readonly ICallHandler _propertySetterHandler;
        private readonly ICallHandler _recordingCallHandler;

        public RecordReplayRoute(ICallHandler eventSubscriptionHandler, ICallHandler propertySetterHandler, ICallHandler recordingCallHandler)
        {
            _eventSubscriptionHandler = eventSubscriptionHandler;
            _propertySetterHandler = propertySetterHandler;
            _recordingCallHandler = recordingCallHandler;
        }

        public object Handle(ICall call)
        {
            _eventSubscriptionHandler.Handle(call);
            _propertySetterHandler.Handle(call);
            return _recordingCallHandler.Handle(call); 
        }
    }
}