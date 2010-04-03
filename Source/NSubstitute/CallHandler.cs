using System;
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
        Func<ICall, object> _handleCall;

        public CallHandler(ICallStack callStack, ICallResults callResults, IReflectionHelper reflectionHelper, ISubstitutionContext context, ICallSpecificationFactory callSpecificationFactory)
        {
            _recordedCallsStack = callStack;
            _callResults = callResults;
            _reflectionHelper = reflectionHelper;
            _context = context;
            _callSpecificationFactory = callSpecificationFactory;
            _handleCall = RecordCall;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _recordedCallsStack.Pop();
            var lastCallSpecification = CallSpecificationFrom(lastCall);
            _callResults.SetResult(lastCallSpecification, valueToReturn);
        }

        public object Handle(ICall call)
        {
            var result = _handleCall(call);
            _handleCall = RecordCall;
            return result;
        }

        object RecordCall(ICall call)
        {
            SetResultForProperty(call);
            _recordedCallsStack.Push(call);
            _context.LastCallHandler(this);
            return _callResults.GetResult(call);
        }

        void SetResultForProperty(ICall call)
        {
            if (_reflectionHelper.IsCallToSetAReadWriteProperty(call))
            {
                var callToPropertyGetter = _reflectionHelper.CreateCallToPropertyGetterFromSetterCall(call);
                var valueBeingSetOnProperty = call.GetArguments().First();
                var callToPropertyGetterSpecification = CallSpecificationFrom(callToPropertyGetter);
                _callResults.SetResult(callToPropertyGetterSpecification, valueBeingSetOnProperty);
            }
        }

        object RaiseEvent(ICall call, object[] eventArguments)
        {
            _reflectionHelper.RaiseEventFromEventAssignment(call, eventArguments);
            return null;
        }

        object CheckCallReceived(ICall call)
        {
            _recordedCallsStack.ThrowIfCallNotFound(CallSpecificationFrom(call));
            return _callResults.GetDefaultResultFor(call);
        }

        ICallSpecification CallSpecificationFrom(ICall call)
        {
            return _callSpecificationFactory.Create(call);
        }

        public void AssertNextCallHasBeenReceived()
        {
            _handleCall = CheckCallReceived;
        }

        public void RaiseEventFromNextCall(params object[] argumentsToRaiseEventWith)
        {
            _handleCall = call => RaiseEvent(call, argumentsToRaiseEventWith);
        }
    }
}