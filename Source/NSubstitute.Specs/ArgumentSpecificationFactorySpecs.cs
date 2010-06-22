using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ArgumentSpecificationFactorySpecs
    {
        public abstract class BaseConcern : ConcernFor<ArgumentSpecificationFactory>
        {
            protected IList<IArgumentSpecification> _argumentSpecifications;
            protected object[] _arguments;
            protected ParameterInfo[] _parameterInfos;
            protected bool _matchAnyArguments;

            public override void Context()
            {
                base.Context();
                _arguments = new object[] { 1, "fred", "harry" };
                _parameterInfos = new ParameterInfo[_arguments.Length];
                int i = 0;
                foreach (var argument in _arguments)
                {
                    var parameterInfo = mock<ParameterInfo>();
                    Type t;
                    parameterInfo.stub(x => t = x.ParameterType).Return(argument.GetType());
                    _parameterInfos[i] = parameterInfo;
                    i++;
                }
                _argumentSpecifications = new List<IArgumentSpecification>();
            }

            public override ArgumentSpecificationFactory CreateSubjectUnderTest()
            {
                return new ArgumentSpecificationFactory();
            }
        }

        public abstract class When_creating_argument_specifications : BaseConcern
        {
            protected IEnumerable<IArgumentSpecification> _result;

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterInfos, _matchAnyArguments);
            }
            
            [Test]
            public void Should_have_specifications_for_all_arguments()
            {
                Assert.That(_result.Count(), Is.EqualTo(_arguments.Length));
            }
        }

        public class When_creating_argument_specifications_with_no_argument_specifications_given : When_creating_argument_specifications
        {
            [Test]
            public void Should_set_argument_specifications_that_match_arguments()
            {
                for (int i = 0; i < _arguments.Count(); i++)
                {
                    Assert.That(_result.ElementAt(i), Is.TypeOf(typeof(ArgumentEqualsSpecification)));
                    Assert.That(_result.ElementAt(i).IsSatisfiedBy(_arguments[i]));
                    Assert.That(_result.ElementAt(i).IsSatisfiedBy("some other argument"), Is.False);
                }
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

        public class When_creating_argument_specifications_with_argument_specs_for_each_argument_of_that_type : When_creating_argument_specifications
        {
            public override void Context()
            {
                base.Context();
                var argumentSpecification = mock<IArgumentSpecification>();
                Type t;
                argumentSpecification.stub(x => t = x.ForType).Return(_arguments[0].GetType());
                _argumentSpecifications.Add(argumentSpecification);
            }

            [Test]
            public void Should_use_argument_spec_for_argument_matching_type()
            {
                Assert.That(_result.ElementAt(0), Is.EqualTo(_argumentSpecifications[0]));
            }
        }

        public class When_creating_argument_specifications_with_less_argument_specs_than_arguments_of_that_type : BaseConcern
        {
            public override void Context()
            {
                base.Context();
                var argumentSpecification = mock<IArgumentSpecification>();
                Type t;
                argumentSpecification.stub(x => t = x.ForType).Return(_arguments[1].GetType());
                _argumentSpecifications.Add(argumentSpecification);
            }

            [Test]
            public void Should_throw_amgiguous_arguments_exception()
            {
                Assert.Throws<AmbiguousArgumentsException>(
                        () => sut.Create(_argumentSpecifications, _arguments, _parameterInfos, false)
                    );
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