using System;

namespace NSubstitute
{
    public class CallRouter : ICallRouter
    {
        readonly IReflectionHelper _reflectionHelper;
        readonly ISubstitutionContext _context;
        readonly ICallHandler _recordingCallHandler;
        readonly ICallHandler _checkReceivedCallHandler;
        private readonly IResultSetter _resultSetter;
        Func<ICall, object> _handleCall;

        public CallRouter(IReflectionHelper reflectionHelper, ISubstitutionContext context, 
            ICallHandler recordingCallHandler, ICallHandler checkReceivedCallHandler, IResultSetter resultSetter)
        {
            _reflectionHelper = reflectionHelper;
            _context = context;
            _recordingCallHandler = recordingCallHandler;
            _checkReceivedCallHandler = checkReceivedCallHandler;
            _resultSetter = resultSetter;
            _handleCall = _recordingCallHandler.Handle;
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
            _handleCall = _recordingCallHandler.Handle;
        }

        public void AssertNextCallHasBeenReceived()
        {
            _handleCall = _checkReceivedCallHandler.Handle;
        }

        public void RaiseEventFromNextCall(params object[] argumentsToRaiseEventWith)
        {
            _handleCall = call => RaiseEvent(call, argumentsToRaiseEventWith);
        }

        private object RaiseEvent(ICall call, object[] eventArguments)
        {
            _reflectionHelper.RaiseEventFromEventAssignment(call, eventArguments);
            return null;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            _resultSetter.SetResultForLastCall(valueToReturn);
        }
    }
}