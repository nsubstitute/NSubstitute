using System.Linq;

namespace NSubstitute
{
    public class CallHandler : ICallHandler
    {
        readonly ICallStack _recordedCallsStack;
        readonly ICallResults _callResults;
        readonly IReflectionHelper _reflectionHelper;
        readonly ISubstitutionContext _context;
        readonly ICallSpecificationFactory _callSpecificationFactory;
        bool _assertNextCallReceived;
        private object[] _eventArgumentsForNextCall;
        private bool _raiseEventFromNextCall;

        public CallHandler(ICallStack callStack, ICallResults callResults, IReflectionHelper reflectionHelper, ISubstitutionContext context, ICallSpecificationFactory callSpecificationFactory)
        {
            _recordedCallsStack = callStack;
            _callResults = callResults;
            _reflectionHelper = reflectionHelper;
            _context = context;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _recordedCallsStack.Pop();
            var lastCallSpecification = CallSpecificationFrom(lastCall);
            _callResults.SetResult(lastCallSpecification, valueToReturn);
        }

        public object Handle(ICall call)
        {
            if (_assertNextCallReceived)
            {
                _assertNextCallReceived = false;
                _recordedCallsStack.ThrowIfCallNotFound(CallSpecificationFrom(call));
                return _callResults.GetDefaultResultFor(call);
            }
            if (_raiseEventFromNextCall)
            {
                _reflectionHelper.RaiseEventFromEventAssignment(call, _eventArgumentsForNextCall);
                _raiseEventFromNextCall = false;
                _eventArgumentsForNextCall = null;
                return null;
            }
            if (_reflectionHelper.IsCallToSetAReadWriteProperty(call))
            {
                var callToPropertyGetter = _reflectionHelper.CreateCallToPropertyGetterFromSetterCall(call);
                var valueBeingSetOnProperty = call.GetArguments().First();
                var callToPropertyGetterSpecification = CallSpecificationFrom(callToPropertyGetter);
                _callResults.SetResult(callToPropertyGetterSpecification, valueBeingSetOnProperty);
            }
            _recordedCallsStack.Push(call);
            _context.LastCallHandler(this);
            return _callResults.GetResult(call);
        }

        ICallSpecification CallSpecificationFrom(ICall call)
        {
            return _callSpecificationFactory.Create(call);
        }

        public void AssertNextCallHasBeenReceived()
        {
            _assertNextCallReceived = true;
        }

        public void RaiseEventFromNextCall(params object[] argumentsToRaiseEventWith)
        {
            _raiseEventFromNextCall = true;
            _eventArgumentsForNextCall = argumentsToRaiseEventWith;
        }
    }
}