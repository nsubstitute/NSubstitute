using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs.Arguments
{
    public class ArgumentSpecificationsFactorySpecs
    {
        public abstract class When_creating_argument_specifications : ConcernFor<ArgumentSpecificationsFactory>
        {
            protected IEnumerable<IArgumentSpecification> _result;
            protected MatchArgs _matchArgs;
            protected IList<IArgumentSpecification> _mixedArgumentSpecifications;

            private IMixedArgumentSpecificationsFactory _mixedArgumentSpecificationsFactory;
            private IList<IArgumentSpecification> _argumentSpecifications;
            private object[] _arguments;
            private IParameterInfo[] _parameterInfos;

            public override void Context()
            {
                base.Context();
                _arguments = new object[2];
                _parameterInfos = new IParameterInfo[2];
                _argumentSpecifications = new List<IArgumentSpecification>();
                _mixedArgumentSpecifications = new List<IArgumentSpecification>();
                _mixedArgumentSpecificationsFactory = mock<IMixedArgumentSpecificationsFactory>();
                _mixedArgumentSpecificationsFactory
                    .Stub(x => x.Create(_argumentSpecifications, _arguments, _parameterInfos))
                    .Return(_mixedArgumentSpecifications);
            }

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterInfos, _matchArgs);
            }

            public override ArgumentSpecificationsFactory CreateSubjectUnderTest()
            {
                return new ArgumentSpecificationsFactory(_mixedArgumentSpecificationsFactory);
            }
        }

        public class When_creating_arg_specs_that_match_arguments_as_specified_in_call : When_creating_argument_specifications
        {
            public override void Context()
            {
                base.Context();
                _matchArgs = MatchArgs.AsSpecifiedInCall;
            }

            [Test]
            public void Should_return_arg_specs_as_worked_out_by_mixed_argument_specification_factory()
            {
                Assert.That(_result, Is.SameAs(_mixedArgumentSpecifications));
            }
        }

        public class When_creating_arg_specs_that_match_any_arguments : When_creating_argument_specifications
        {
            private IArgumentSpecification _anyArgsVersionOfIntSpec;
            private IArgumentSpecification _anyArgsVersionOfStringSpec;

            public override void Context()
            {
                base.Context();
                _matchArgs = MatchArgs.Any;

                _anyArgsVersionOfIntSpec = mock<IArgumentSpecification>();
                _anyArgsVersionOfStringSpec = mock<IArgumentSpecification>();

                _mixedArgumentSpecifications.Add(CreateSpecWith(typeof(int), _anyArgsVersionOfIntSpec));
                _mixedArgumentSpecifications.Add(CreateSpecWith(typeof(string), _anyArgsVersionOfStringSpec));
            }

            [Test]
            public void Should_return_arg_specs_based_on_those_worked_out_by_mixed_arg_spec_factory()
            {
                var resultArray = _result.ToArray();
                Assert.That(resultArray[0], Is.EqualTo(_anyArgsVersionOfIntSpec), "Result should be a copy of original spec that matches any args");
                Assert.That(resultArray[1], Is.SameAs(_anyArgsVersionOfStringSpec), "Result should be a copy of original spec that matches any args");
            }

            private IArgumentSpecification CreateSpecWith(Type type, IArgumentSpecification anyArgsVersionOfSpec)
            {
                var spec = mock<IArgumentSpecification>();
                spec.stub(x => x.ForType).Return(type);
                spec.stub(x => x.CreateCopyMatchingAnyArgOfType(type)).Return(anyArgsVersionOfSpec);
                return spec;
            }
        }
    }
}