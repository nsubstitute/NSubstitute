using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class SubstituteState : ISubstituteState
    {
        private readonly IEnumerable<object> _state;

        public SubstituteState(IEnumerable<object> state)
        {
            _state = state;
        }

        public static SubstituteState Create(ISubstitutionContext substitutionContext)
        {
            var callInfoFactory = new CallInfoFactory();
            var callStack = new CallStack();
            var callResults = new CallResults(callInfoFactory);
            var callSpecificationFactory = new CallSpecificationFactory(substitutionContext, new ArgumentSpecificationFactory(new MixedArgumentSpecificationFactory()));

            var callFormatter = new CallFormatter(new ArgumentsFormatter(new ArgumentFormatter()));

            var state = new object[] 
            {
                callInfoFactory,
                callStack,
                callResults,
                callSpecificationFactory,
                new CallActions(),
                new PropertyHelper(),
                new ResultSetter(callStack, callResults, callSpecificationFactory),
                new EventHandlerRegistry(),
                new CallNotReceivedExceptionThrower(callFormatter),
                new CallReceivedExceptionThrower(callFormatter),
                new DefaultForType()
            };

            return new SubstituteState(state);
        }

        public SubstituteState(ISubstitutionContext substitutionContext)
        {
            CallInfoFactory = new CallInfoFactory();
            CallStack = new CallStack();
            CallResults = new CallResults(CallInfoFactory);
            CallActions = new CallActions();
            PropertyHelper = new PropertyHelper();
            CallSpecificationFactory = new CallSpecificationFactory(substitutionContext, new ArgumentSpecificationFactory(new MixedArgumentSpecificationFactory()));
            ResultSetter = new ResultSetter(CallStack, CallResults, CallSpecificationFactory);
            EventHandlerRegistry = new EventHandlerRegistry();
            CallNotReceivedExceptionThrower = new CallNotReceivedExceptionThrower(new CallFormatter(new ArgumentsFormatter(new ArgumentFormatter())));
            CallReceivedExceptionThrower = new CallReceivedExceptionThrower(new CallFormatter(new ArgumentsFormatter(new ArgumentFormatter())));
            DefaultForType = new DefaultForType();
        }

        public CallStack CallStack { get; private set; }
        public ICallResults CallResults { get; private set; }
        public IPropertyHelper PropertyHelper { get; private set; }
        public ICallSpecificationFactory CallSpecificationFactory { get; private set; }
        public IResultSetter ResultSetter { get; private set; }
        public IEventHandlerRegistry EventHandlerRegistry { get; private set; }
        public ICallActions CallActions { get; private set; }
        public CallInfoFactory CallInfoFactory { get; private set; }
        public ICallNotReceivedExceptionThrower CallNotReceivedExceptionThrower { get; private set; }
        public ICallReceivedExceptionThrower CallReceivedExceptionThrower { get; private set; }
        public IDefaultForType DefaultForType { get; private set; }

        public object FindInstanceFor(Type type, object[] additionalArguments)
        {
            return _state
                    .Concat(additionalArguments ?? new object[0])
                    .FirstOrDefault(x => type.IsAssignableFrom(x.GetType()));
        }
    }
}