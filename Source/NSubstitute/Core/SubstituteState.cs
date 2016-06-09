﻿using System.Collections.Generic;
using System.Linq;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Routing.Handlers;

namespace NSubstitute.Core
{
    public class SubstituteState : ISubstituteState
    {
        public ISubstitutionContext SubstitutionContext { get; private set; }
        public ICallStack CallStack { get; private set; }
        public IReceivedCalls ReceivedCalls { get; private set; }
        public IPendingSpecification PendingSpecification { get; private set; }
        public ICallResults CallResults { get; private set; }
        public ICallSpecificationFactory CallSpecificationFactory { get; private set; }
        public ICallActions CallActions { get; private set; }
        public ICallBaseExclusions CallBaseExclusions { get; private set; }
        public SubstituteConfig SubstituteConfig { get; set; }
        public SequenceNumberGenerator SequenceNumberGenerator { get; private set; }
        public IConfigureCall ConfigureCall { get; private set; }
        public IEventHandlerRegistry EventHandlerRegistry { get; private set; }
        public IList<IAutoValueProvider> AutoValueProviders { get; private set; }
        public IResultsForType ResultsForType { get; private set; }
        public AutoValueBehaviour AutoValueBehaviour { get; set; }

        public SubstituteState(ISubstitutionContext substitutionContext, SubstituteConfig option)
        {
            SubstitutionContext = substitutionContext;
            SubstituteConfig = option;
            SequenceNumberGenerator = substitutionContext.SequenceNumberGenerator;
            var substituteFactory = substitutionContext.SubstituteFactory;
            var callInfoFactory = new CallInfoFactory();
            var callStack = new CallStack();
            CallStack = callStack;
            ReceivedCalls = callStack;
            PendingSpecification = new PendingSpecification();
            CallResults = new CallResults(callInfoFactory);
            CallSpecificationFactory = CallSpecificationFactoryFactoryYesThatsRight.CreateCallSpecFactory();
            CallActions = new CallActions(callInfoFactory);
            CallBaseExclusions = new CallBaseExclusions();
            ResultsForType = new ResultsForType(callInfoFactory);

            var getCallSpec = new GetCallSpec(callStack, PendingSpecification, CallSpecificationFactory, CallActions);

            ConfigureCall = new ConfigureCall(CallResults, CallActions, getCallSpec);
            EventHandlerRegistry = new EventHandlerRegistry();
            AutoValueProviders = new List<IAutoValueProvider> { 
#if NET45
                new AutoObservableProvider(() => new CompositeProvider(AutoValueProviders.ToArray())),
#endif
                new AutoQueryableProvider(),
                new AutoSubstituteProvider(substituteFactory), 
                new AutoStringProvider(), 
                new AutoArrayProvider(),
#if (NET4 || NET45)
                new AutoTaskProvider(() => new CompositeProvider(AutoValueProviders.ToArray())),
#endif
            };
        }

        public void ClearUnusedCallSpecs()
        {
            PendingSpecification.Clear();
        }
    }
}
