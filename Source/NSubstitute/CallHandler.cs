using System.Collections.Generic;
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

        public void LastCallShouldReturn<T>(T valueToReturn, IList<IArgumentMatcher> argumentMatchers)
        {
            var lastCall = _recordedCallsStack.Pop();
            var lastCallSpecification = CallSpecificationFrom(lastCall, argumentMatchers);
            _callResults.SetResult(lastCallSpecification, valueToReturn);
        }

        public object Handle(ICall call, IList<IArgumentMatcher> argumentMatchers)
        {
            var callSpecification = CallSpecificationFrom(call, argumentMatchers);
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
                var callToPropertyGetterSpecification = CallSpecificationFrom(callToPropertyGetter, argumentMatchers);
                _callResults.SetResult(callToPropertyGetterSpecification, valueBeingSetOnProperty);
            }
            _recordedCallsStack.Push(call);
            _context.LastCallHandler(this);
            return _callResults.GetResult(call);
        }

        ICallSpecification CallSpecificationFrom(ICall call, IList<IArgumentMatcher> argumentMatchers)
        {
            return _callSpecificationFactory.Create(call, argumentMatchers);
        }

        public void AssertNextCallHasBeenReceived()
        {
            _assertNextCallReceived = true;
        }
    }
}