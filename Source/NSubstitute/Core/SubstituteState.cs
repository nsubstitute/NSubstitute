using NSubstitute.Routing.AutoValues;
using NSubstitute.Core.Arguments;

namespace NSubstitute.Core
{
    public class SubstituteState : ISubstituteState
    {
        public ISubstitutionContext SubstitutionContext { get; private set; }
        public ICallInfoFactory CallInfoFactory { get; private set; }
        public ICallStack CallStack { get; private set; }
        public IReceivedCalls ReceivedCalls { get; private set; }
        public IPendingSpecification PendingSpecification { get; private set; }
        public ICallResults CallResults { get; private set; }
        public ICallSpecificationFactory CallSpecificationFactory { get; private set; }
        public ISubstituteFactory SubstituteFactory { get; private set; }
        public ICallActions CallActions { get; private set; }
        public SequenceNumberGenerator SequenceNumberGenerator { get; private set; }
        public IPropertyHelper PropertyHelper { get; private set; }
        public IResultSetter ResultSetter { get; private set; }
        public IEventHandlerRegistry EventHandlerRegistry { get; private set; }
        public IReceivedCallsExceptionThrower ReceivedCallsExceptionThrower { get; private set; }
        public IDefaultForType DefaultForType { get; private set; }
        public IAutoValueProvider[] AutoValueProviders { get; private set; }

        public SubstituteState(ISubstitutionContext substitutionContext)
        {
            SubstitutionContext = substitutionContext;
            SubstituteFactory = substitutionContext.SubstituteFactory;
            SequenceNumberGenerator = substitutionContext.SequenceNumberGenerator;
            CallInfoFactory = new CallInfoFactory();
            var callStack = new CallStack();
            CallStack = callStack;
            ReceivedCalls = callStack;
            PendingSpecification = new PendingSpecification();
            CallResults = new CallResults(CallInfoFactory);
            CallSpecificationFactory = NewCallSpecificationFactory();
            CallActions = new CallActions(CallInfoFactory);

            PropertyHelper = new PropertyHelper();
            ResultSetter = new ResultSetter(CallStack, PendingSpecification, CallResults, CallSpecificationFactory, CallActions);
            EventHandlerRegistry = new EventHandlerRegistry();
            ReceivedCallsExceptionThrower = new ReceivedCallsExceptionThrower();
            DefaultForType = new DefaultForType();
            AutoValueProviders = new IAutoValueProvider[] { 
#if NET4
                new AutoTaskProvider(() => AutoValueProviders),
#endif
                new AutoSubstituteProvider(SubstituteFactory), 
                new AutoStringProvider(), 
                new AutoArrayProvider() 
            };
        }

        private static IDefaultChecker NewDefaultChecker()
        {
            return new DefaultChecker(new DefaultForType());
        }

        private static IParamsArgumentSpecificationFactory NewParamsArgumentSpecificationFactory()
        {
            return
                new ParamsArgumentSpecificationFactory(
                    NewDefaultChecker(),
                    new ArgumentEqualsSpecificationFactory(),
                    new ArrayArgumentSpecificationsFactory(
                        new NonParamsArgumentSpecificationFactory(new ArgumentEqualsSpecificationFactory()
                        )
                    ),
                    new ParameterInfosFromParamsArrayFactory(),
                    new SuppliedArgumentSpecificationsFactory(NewDefaultChecker()),
                    new ArrayContentsArgumentSpecificationFactory()
                );
        }

        private static INonParamsArgumentSpecificationFactory NewNonParamsArgumentSpecificationFactory()
        {
            return
                new NonParamsArgumentSpecificationFactory(new ArgumentEqualsSpecificationFactory()
                );
        }

        private static ICallSpecificationFactory NewCallSpecificationFactory()
        {
            return
                new CallSpecificationFactory(
                    new ArgumentSpecificationsFactory(
                        new MixedArgumentSpecificationsFactory(
                            new ArgumentSpecificationFactory(
                                NewParamsArgumentSpecificationFactory(),
                                NewNonParamsArgumentSpecificationFactory()
                            ),
                            new SuppliedArgumentSpecificationsFactory(NewDefaultChecker())
                        )
                    )
                );
        }
    }
}
