using System.Linq;

namespace NSubstitute
{
    public class CallHandler : ICallHandler
    {
        readonly ICallStack _recordedCallsStack;
        readonly ICallResults _callResults;
        readonly IPropertyHelper _propertyHelper;
        readonly ISubstitutionContext _context;
        bool _assertNextCallReceived;

        public CallHandler(ICallStack callStack, ICallResults callResults, IPropertyHelper propertyHelper, ISubstitutionContext context)
        {
            _recordedCallsStack = callStack;
            _callResults = callResults;
            _propertyHelper = propertyHelper;
            _context = context;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _recordedCallsStack.Pop();
            _callResults.SetResult(lastCall, valueToReturn);
        }

        public object Handle(ICall call)
        {
            if (_assertNextCallReceived)
            {
                _assertNextCallReceived = false;
                _recordedCallsStack.ThrowIfCallNotFound(call);
                return _callResults.GetDefaultResultFor(call);
            }
            if (_propertyHelper.IsCallToSetAReadWriteProperty(call))
            {
                var callToPropertyGetter = _propertyHelper.CreateCallToPropertyGetterFromSetterCall(call);
                var valueBeingSetOnProperty = call.GetArguments().First();
                _callResults.SetResult(callToPropertyGetter, valueBeingSetOnProperty);
            }
            _recordedCallsStack.Push(call);
            _context.LastCallHandler(this);
            return _callResults.GetResult(call);
        }

        public void AssertNextCallHasBeenReceived()
        {
            _assertNextCallReceived = true;
        }
    }
}