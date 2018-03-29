using System.Collections.Generic;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public class SubstituteState : ISubstituteState
    {
        public ICallCollection ReceivedCalls { get; }
        public ICallResults CallResults { get; }
        public ICallActions CallActions { get; }
        public ICallBaseExclusions CallBaseExclusions { get; }
        public SubstituteConfig SubstituteConfig { get; set; }
        public SequenceNumberGenerator SequenceNumberGenerator { get; }
        public IConfigureCall ConfigureCall { get; }
        public IEventHandlerRegistry EventHandlerRegistry { get; }
        public IReadOnlyCollection<IAutoValueProvider> AutoValueProviders { get; }
        public ICallResults AutoValuesCallResults { get; }
        public IResultsForType ResultsForType { get; }
        public ICustomHandlers CustomHandlers { get; }

        public SubstituteState(
            SubstituteConfig option,
            SequenceNumberGenerator sequenceNumberGenerator,
            ICallSpecificationFactory callSpecificationFactory,
            ICallInfoFactory callInfoFactory,
            IReadOnlyCollection<IAutoValueProvider> autoValueProviders)
        {
            SubstituteConfig = option;
            SequenceNumberGenerator = sequenceNumberGenerator;
            AutoValueProviders = autoValueProviders;

            var callCollection = new CallCollection();
            ReceivedCalls = callCollection;

            CallResults = new CallResults(callInfoFactory);
            AutoValuesCallResults = new CallResults(callInfoFactory);
            CallActions = new CallActions(callInfoFactory);
            CallBaseExclusions = new CallBaseExclusions();
            ResultsForType = new ResultsForType(callInfoFactory);
            CustomHandlers = new CustomHandlers(this);
            var getCallSpec = new GetCallSpec(callCollection, callSpecificationFactory, CallActions);
            ConfigureCall = new ConfigureCall(CallResults, CallActions, getCallSpec);
            EventHandlerRegistry = new EventHandlerRegistry();

        }
    }
}
