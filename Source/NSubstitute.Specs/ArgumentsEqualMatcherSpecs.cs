using System;
using System.Collections;
using System.Collections.Generic;
using NSubstitute.Specs.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ArgumentsEqualMatcherSpecs
    {
        public abstract class Concern : ConcernFor<ArgumentsEqualMatcher>
        {
            protected IInvocation _first;
            protected IInvocation _second;
            protected IArgumentEqualityComparer _equalityComparer;
            protected bool _result;

            public override void Because()
            {
                _result = sut.IsMatch(_first, _second);
            }

            public override void Context()
            {
                _first = mock<IInvocation>();
                _second = mock<IInvocation>();
                _equalityComparer = mock<IArgumentEqualityComparer>();
            }

            public override ArgumentsEqualMatcher CreateSubjectUnderTest()
            {
                return new ArgumentsEqualMatcher(_equalityComparer);
            }

            protected void SetupInvocationArguments(IInvocation invocation, object[] arguments)
            {
                invocation.stub(x => x.GetArguments()).Return(arguments);
            }
        }

        public class When_invocations_have_the_same_number_of_arguments_and_they_all_are_equal : Concern
        {
            public override void Context()
            {
                base.Context();
                _equalityComparer.stub(x => x.Equals(null, null)).IgnoreArguments().Return(true);
                SetupInvocationArguments(_first, new object[2]);
                SetupInvocationArguments(_second, new object[2]);
            }

            [Test]
            public void Should_match()
            {
                Assert.That(_result);
            }
        }

        public class When_invocations_have_a_different_number_of_arguments_and_the_ones_provided_are_equal : Concern
        {
            public override void Context()
            {
                base.Context();
                _equalityComparer.stub(x => x.Equals(null, null)).IgnoreArguments().Return(true);
                SetupInvocationArguments(_first, new object[2]);
                SetupInvocationArguments(_second, new object[3]);
            }

            [Test]
            public void Should_not_match()
            {
                Assert.That(_result, Is.False);
            }
        }

        public class When_invocations_have_the_same_number_of_arguments_and_they_are_different : Concern
        {
            public override void Context()
            {
                base.Context();
                _equalityComparer.stub(x => x.Equals(null, null)).IgnoreArguments().Return(false);
                SetupInvocationArguments(_first, new object[2]);
                SetupInvocationArguments(_second, new object[2]);
            }

            [Test]
            public void Should_not_match()
            {
                Assert.That(_result, Is.False);
            }
        }

    }
}