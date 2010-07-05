using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs
{
    public class ArgumentSpecificationFactorySpecs
    {
        public abstract class BaseConcern : ConcernFor<ArgumentSpecificationFactory>
        {
            protected IList<IArgumentSpecification> _argumentSpecifications;
            protected object[] _arguments;
            protected Type[] _parameterTypes;
            protected bool _matchAnyArguments;
            protected IMixedArgumentSpecificationFactory _mixedArgumentSpecificationFactory;

            public override void Context()
            {
                base.Context();
                _arguments = new object[] { 1, "fred", "harry" };
                _parameterTypes = _arguments.Select(x => x.GetType()).ToArray();
                _argumentSpecifications = new List<IArgumentSpecification>();
                _mixedArgumentSpecificationFactory = mock<IMixedArgumentSpecificationFactory>();
            }

            public override ArgumentSpecificationFactory CreateSubjectUnderTest()
            {
                return new ArgumentSpecificationFactory(_mixedArgumentSpecificationFactory);
            }
        }

        public abstract class When_creating_argument_specifications : BaseConcern
        {
            protected IEnumerable<IArgumentSpecification> _result;

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterTypes, _matchAnyArguments);
            }
            
            [Test]
            public void Should_have_specifications_for_all_arguments()
            {
                Assert.That(_result.Count(), Is.EqualTo(_arguments.Length));
            }
        }

        public class When_creating_argument_specifications_with_argument_specs_for_each_argument : When_creating_argument_specifications
        {
            public override void Context()
            {
                base.Context();
                foreach (var argument in _arguments)
                {
                    _argumentSpecifications.Add(mock<IArgumentSpecification>());                    
                }
            }

            [Test]
            public void Should_use_argument_specs_passed_in()
            {
                Assert.That(_result, Is.EquivalentTo(_argumentSpecifications));
            }
        }

        public class When_creating_argument_specifications_with_no_argument_specifications_given : BaseConcern
        {
            protected IEnumerable<IArgumentSpecification> _mixedArgumentSpecs;
            protected IEnumerable<IArgumentSpecification> _result;

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterTypes, _matchAnyArguments);
            }

            public override void Context()
            {
                base.Context();
                _mixedArgumentSpecs = mock<IEnumerable<IArgumentSpecification>>();
                _mixedArgumentSpecificationFactory.Stub(
                    x => x.Create(_argumentSpecifications, _arguments, _parameterTypes)).Return(_mixedArgumentSpecs);
            }

            [Test]
            public void Should_use_mixed_argument_specifcation_factory()
            {
                Assert.That(_result == _mixedArgumentSpecs);
            }
        }

        public class When_creating_argument_specifications_with_less_argument_specs_than_arguments : When_creating_argument_specifications_with_no_argument_specifications_given
        {
            public override void Context()
            {
                base.Context();
                var argumentSpecification = mock<IArgumentSpecification>();
                Type t;
                argumentSpecification.stub(x => t = x.ForType).Return(_parameterTypes[0]);
                _argumentSpecifications.Add(argumentSpecification);
            }
        }

        public class When_creating_argument_specifications_that_match_any_arguments : When_creating_argument_specifications
        {
            public override void Context()
            {
                base.Context();
                _matchAnyArguments = true;
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