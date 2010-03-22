using System;
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
        ICallSpecificationFactory _callSpecificationFactory;

        public CallHandler(ICallStack callStack, ICallResults callResults, IPropertyHelper propertyHelper, ISubstitutionContext context, ICallSpecificationFactory callSpecificationFactory)
        {
            _recordedCallsStack = callStack;
            _callResults = callResults;
            _propertyHelper = propertyHelper;
            _context = context;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public void LastCallShouldReturn<T>(T valueToReturn)
        {
            var lastCall = _recordedCallsStack.Pop();
            _callResults.SetResult(lastCall, valueToReturn);
        }

        public object Handle(ICall call)
        {
            var callSpecification = CallSpecificationFrom(call);
            if (_assertNextCallReceived)
            {
                _assertNextCallReceived = false;
                _recordedCallsStack.ThrowIfCallNotFound(callSpecification);
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

        ICallSpecification CallSpecificationFrom(ICall call)
        {
            return _callSpecificationFactory.Create(call);
        }

        public void AssertNextCallHasBeenReceived()
        {
            _assertNextCallReceived = true;
        }
    }
}