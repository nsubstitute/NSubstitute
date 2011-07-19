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
        public abstract class BaseConcern : ConcernFor<ArgumentSpecificationsFactory>
        {
            protected IList<IArgumentSpecification> _argumentSpecifications;
            protected object[] _arguments;
            protected MatchArgs _matchArgs;
            protected IMixedArgumentSpecificationsFactory MixedArgumentSpecificationsFactory;
            protected IParameterInfo[] _parameterInfos;

            public override void Context()
            {
                base.Context();
                _arguments = new object[] { 1, "fred", "harry" };
                _parameterInfos = new[] { mock<IParameterInfo>(), mock<IParameterInfo>(), mock<IParameterInfo>() };
                _parameterInfos[0].stub(x => x.ParameterType).Return(_arguments[0].GetType());
                _parameterInfos[1].stub(x => x.ParameterType).Return(_arguments[1].GetType());
                _parameterInfos[2].stub(x => x.ParameterType).Return(_arguments[2].GetType());
                _argumentSpecifications = new List<IArgumentSpecification>();
                MixedArgumentSpecificationsFactory = mock<IMixedArgumentSpecificationsFactory>();
            }

            public override ArgumentSpecificationsFactory CreateSubjectUnderTest()
            {
                return new ArgumentSpecificationsFactory(MixedArgumentSpecificationsFactory);
            }
        }

        public abstract class When_creating_argument_specifications : BaseConcern
        {
            protected IEnumerable<IArgumentSpecification> _result;

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterInfos, _matchArgs);
            }

            [Test]
            public void Should_have_specifications_for_all_arguments()
            {
                Assert.That(_result.Count(), Is.EqualTo(_arguments.Length));
            }
        }

        public class When_creating_argument_specifications_with_no_argument_specifications_given : BaseConcern
        {
            protected IEnumerable<IArgumentSpecification> _mixedArgumentSpecs;
            protected IEnumerable<IArgumentSpecification> _result;

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterInfos, _matchArgs);
            }

            public override void Context()
            {
                base.Context();
                _mixedArgumentSpecs = mock<IEnumerable<IArgumentSpecification>>();
                MixedArgumentSpecificationsFactory.Stub(
                    x => x.Create(_argumentSpecifications, _arguments, _parameterInfos)).Return(_mixedArgumentSpecs);
            }

            [Test]
            public void Should_use_mixed_argument_specifcation_factory()
            {
                Assert.That(_result == _mixedArgumentSpecs);
            }
        }

        public class When_creating_argument_specifications_with_less_argument_specs_than_arguments : When_creating_argument_specifications_with_no_argument_specifications_given
        {
            protected Type _ignored;

            public override void Context()
            {
                base.Context();
                var argumentSpecification = mock<IArgumentSpecification>();
                argumentSpecification.stub(x => _ignored = x.ForType).Return(_parameterInfos[0].ParameterType);
                _argumentSpecifications.Add(argumentSpecification);
            }
        }

        public class When_creating_argument_specifications_that_match_any_arguments : When_creating_argument_specifications
        {
            public override void Context()
            {
                base.Context();
                _matchArgs = MatchArgs.Any;
            }

            [Test]
            public void Should_return_specifications_that_match_any_arguments()
            {
                for (int i = 0; i < _arguments.Count(); i++)
                {
                    Assert.That(_result.ElementAt(i), Is.TypeOf(typeof(ArgumentIsAnythingSpecification)));
                    Assert.That(_result.ElementAt(i).ForType, Is.EqualTo(_arguments[i].GetType()));
                }
            }
        }
    }
}