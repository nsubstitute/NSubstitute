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
            protected ICall _first;
            protected ICall _second;
            protected IArgumentEqualityComparer _equalityComparer;
            protected bool _result;

            public override void Because()
            {
                _result = sut.IsMatch(_first, _second);
            }

            public override void Context()
            {
                _first = mock<ICall>();
                _second = mock<ICall>();
                _equalityComparer = mock<IArgumentEqualityComparer>();
            }

            public override ArgumentsEqualMatcher CreateSubjectUnderTest()
            {
                return new ArgumentsEqualMatcher(_equalityComparer);
            }

            protected void SetupCallArguments(ICall call, object[] arguments)
            {
                call.stub(x => x.GetArguments()).Return(arguments);
            }
        }

        public class When_calls_have_the_same_number_of_arguments_and_they_all_are_equal : Concern
        {
            public override void Context()
            {
                base.Context();
                _equalityComparer.stub(x => x.Equals(null, null)).IgnoreArguments().Return(true);
                SetupCallArguments(_first, new object[2]);
                SetupCallArguments(_second, new object[2]);
            }

            [Test]
            public void Should_match()
            {
                Assert.That(_result);
            }
        }

        public class When_calls_have_a_different_number_of_arguments_and_the_ones_provided_are_equal : Concern
        {
            public override void Context()
            {
                base.Context();
                _equalityComparer.stub(x => x.Equals(null, null)).IgnoreArguments().Return(true);
                SetupCallArguments(_first, new object[2]);
                SetupCallArguments(_second, new object[3]);
            }

            [Test]
            public void Should_not_match()
            {
                Assert.That(_result, Is.False);
            }
        }

        public class When_calls_have_the_same_number_of_arguments_and_they_are_different : Concern
        {
            public override void Context()
            {
                base.Context();
                _equalityComparer.stub(x => x.Equals(null, null)).IgnoreArguments().Return(false);
                SetupCallArguments(_first, new object[2]);
                SetupCallArguments(_second, new object[2]);
            }

            [Test]
            public void Should_not_match()
            {
                Assert.That(_result, Is.False);
            }
        }

    }
}