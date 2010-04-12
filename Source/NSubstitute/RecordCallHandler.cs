using System;
using System.Linq;

namespace NSubstitute
{
    public class RecordCallHandler : ICallHandler
    {
        private readonly ICallStack _callStack;
        private readonly ICallResults _configuredResults;
        private readonly IReflectionHelper _reflectionHelper;
        private readonly ICallSpecificationFactory _callSpecificationFactory;

        public RecordCallHandler(ICallStack callStack, ICallResults configuredResults, IReflectionHelper reflectionHelper, ICallSpecificationFactory callSpecificationFactory)
        {
            _callStack = callStack;
            _configuredResults = configuredResults;
            _reflectionHelper = reflectionHelper;
            _callSpecificationFactory = callSpecificationFactory;
        }

        public object Handle(ICall call)
        {
            SetResultForProperty(call);
            _callStack.Push(call);
            return _configuredResults.GetResult(call);
        }

        void SetResultForProperty(ICall call)
        {
            if (_reflectionHelper.IsCallToSetAReadWriteProperty(call))
            {
                var callToPropertyGetter = _reflectionHelper.CreateCallToPropertyGetterFromSetterCall(call);
                var valueBeingSetOnProperty = call.GetArguments().First();
                var callToPropertyGetterSpecification = CallSpecificationFrom(callToPropertyGetter);
                _configuredResults.SetResult(callToPropertyGetterSpecification, valueBeingSetOnProperty);
            }
        }

        private ICallSpecification CallSpecificationFrom(ICall call)
        {
            return _callSpecificationFactory.CreateFrom(call);
        }
    }
}