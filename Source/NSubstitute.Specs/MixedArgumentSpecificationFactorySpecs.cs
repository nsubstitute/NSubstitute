using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class MixedArgumentSpecificationFactorySpecs
    {
        public abstract class BaseConcern : ConcernFor<MixedArgumentSpecificationFactory>
        {
            protected IList<IArgumentSpecification> _argumentSpecifications;
            protected object[] _arguments;
            protected Type[] _parameterTypes;
            protected bool _matchAnyArguments;

            public override void Context()
            {
                base.Context();
                _arguments = new object[] { 1, "fred", null, null };
                _parameterTypes = new[] { typeof(int), typeof(string), typeof(string), typeof(string) };
                _argumentSpecifications = new List<IArgumentSpecification>();
            }

            public override MixedArgumentSpecificationFactory CreateSubjectUnderTest()
            {
                return new MixedArgumentSpecificationFactory();
            }
        }

        public abstract class When_creating_argument_specifications : BaseConcern
        {
            protected IEnumerable<IArgumentSpecification> _result;

            public override void Because()
            {
                _result = sut.Create(_argumentSpecifications, _arguments, _parameterTypes);
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

        public class When_creating_argument_specifications_with_argument_specs_and_non_default_argument_specs : When_creating_argument_specifications
        {
            public override void Context()
            {
                base.Context();
                var argumentSpecification = mock<IArgumentSpecification>();
                Type t;
                argumentSpecification.stub(x => t = x.ForType).Return(_parameterTypes[2]);
                _argumentSpecifications.Add(argumentSpecification);

                argumentSpecification = mock<IArgumentSpecification>();
                argumentSpecification.stub(x => t = x.ForType).Return(_parameterTypes[3]);
                _argumentSpecifications.Add(argumentSpecification);
            }

            [Test]
            public void Should_use_argument_spec_for_third_argument()
            {
                Assert.That(_result.ElementAt(2), Is.EqualTo(_argumentSpecifications[0]));
            }

            [Test]
            public void Should_use_argument_spec_for_fourth_argument()
            {
                Assert.That(_result.ElementAt(3), Is.EqualTo(_argumentSpecifications[1]));
            }
        }

        public class When_creating_argument_specifications_with_less_argument_specs_than_default_arguments_of_that_type : BaseConcern
        {
            public override void Context()
            {
                base.Context();
                var argumentSpecification = mock<IArgumentSpecification>();
                Type t;
                argumentSpecification.stub(x => t = x.ForType).Return(_parameterTypes[2]);
                _argumentSpecifications.Add(argumentSpecification);
            }

            [Test]
            public void Should_throw_amgiguous_arguments_exception()
            {
                Assert.Throws<AmbiguousArgumentsException>(
                        () => sut.Create(_argumentSpecifications, _arguments, _parameterTypes)
                    );
            }
        }
    }
}