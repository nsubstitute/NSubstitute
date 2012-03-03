﻿using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Core.Arguments;

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
            var substituteFactory = substitutionContext.SubstituteFactory;
            var callInfoFactory = new CallInfoFactory();
            var callStack = new CallStack();
            var pendingSpecification = new PendingSpecification();
            var callResults = new CallResults(callInfoFactory);
            var callSpecificationFactory = NewCallSpecificationFactory();
            var callActions = new CallActions(callInfoFactory);

            var callFormatter = new CallFormatter(new ArgumentsFormatter(new ArgumentFormatter()), new ArgumentFormatInfoFactory());

            var state = new object[] 
            {
                callInfoFactory,
                callStack,
                pendingSpecification,
                callResults,
                callSpecificationFactory,
                substituteFactory,
                callActions,
                new PropertyHelper(),
                new ResultSetter(callStack, pendingSpecification, callResults, callSpecificationFactory, callActions),
                new EventHandlerRegistry(),
                new ReceivedCallsExceptionThrower(callFormatter),
                new DefaultForType(),
                new IAutoValueProvider[] { new AutoSubstituteProvider(substituteFactory), new AutoStringProvider(), new AutoArrayProvider()}
            };

            return new SubstituteState(state);
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

        public object FindInstanceFor(Type type, object[] additionalArguments)
        {
            return _state
                    .Concat(additionalArguments ?? new object[0])
                    .FirstOrDefault(x => x != null && type.IsAssignableFrom(x.GetType()));
        }
    }
}
