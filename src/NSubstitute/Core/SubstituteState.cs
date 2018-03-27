using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core
{
    public class SubstituteState : ISubstituteState
    {
        public ICallCollection ReceivedCalls { get; }
        public ICallResults CallResults { get; }
        public ICallSpecificationFactory CallSpecificationFactory { get; }
        public ICallActions CallActions { get; }
        public ICallBaseExclusions CallBaseExclusions { get; }
        public SubstituteConfig SubstituteConfig { get; set; }
        public SequenceNumberGenerator SequenceNumberGenerator { get; }
        public IConfigureCall ConfigureCall { get; }
        public IEventHandlerRegistry EventHandlerRegistry { get; }
        public IAutoValueProvider[] AutoValueProviders { get; }
        public ICallResults AutoValuesCallResults { get; }
        public IResultsForType ResultsForType { get; }
        public ICustomHandlers CustomHandlers { get; }

        public SubstituteState(
            SubstituteConfig option,
            SequenceNumberGenerator sequenceNumberGenerator,
            ISubstituteFactory substituteFactory)
        {
            SubstituteConfig = option;
            SequenceNumberGenerator = sequenceNumberGenerator;
            var callInfoFactory = new CallInfoFactory();

            var callCollection = new CallCollection();
            ReceivedCalls = callCollection;

            CallResults = new CallResults(callInfoFactory);
            AutoValuesCallResults = new CallResults(callInfoFactory);
            CallSpecificationFactory = CallSpecificationFactoryFactoryYesThatsRight.CreateCallSpecFactory();
            CallActions = new CallActions(callInfoFactory);
            CallBaseExclusions = new CallBaseExclusions();
            ResultsForType = new ResultsForType(callInfoFactory);
            CustomHandlers = new CustomHandlers(this);
            var getCallSpec = new GetCallSpec(callCollection, CallSpecificationFactory, CallActions);
            ConfigureCall = new ConfigureCall(CallResults, CallActions, getCallSpec);
            EventHandlerRegistry = new EventHandlerRegistry();

            AutoValueProviders = new IAutoValueProvider[] { 
                new AutoObservableProvider(() => AutoValueProviders),
                new AutoQueryableProvider(),
                new AutoSubstituteProvider(substituteFactory),
                new AutoStringProvider(),
                new AutoArrayProvider(),
                new AutoTaskProvider(() => AutoValueProviders),
            };
        }
    }
}
