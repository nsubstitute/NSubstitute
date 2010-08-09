using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Routing.AutoValues;

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
            var substituteFactory = substitutionContext.GetSubstituteFactory();
            var callInfoFactory = new CallInfoFactory();
            var callStack = new CallStack();
            var callResults = new CallResults(callInfoFactory);
            var callSpecificationFactory = new CallSpecificationFactory(new ArgumentSpecificationFactory(new MixedArgumentSpecificationFactory()));

            var callFormatter = new CallFormatter(new ArgumentsFormatter(new ArgumentFormatter()));

            var state = new object[] 
            {
                callInfoFactory,
                callStack,
                callResults,
                callSpecificationFactory,
                substituteFactory,
                new CallActions(),
                new PropertyHelper(),
                new ResultSetter(callStack, callResults, callSpecificationFactory),
                new EventHandlerRegistry(),
                new CallNotReceivedExceptionThrower(callFormatter),
                new CallReceivedExceptionThrower(callFormatter),
                new DefaultForType(),
                new IAutoValueProvider[] { new AutoSubstituteProvider(substituteFactory), new AutoStringProvider()}
            };

            return new SubstituteState(state);
        }

        public object FindInstanceFor(Type type, object[] additionalArguments)
        {
            return _state
                    .Concat(additionalArguments ?? new object[0])
                    .FirstOrDefault(x => x != null && type.IsAssignableFrom(x.GetType()));
        }
    }
}