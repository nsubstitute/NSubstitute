using System.Collections.Generic;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs
{
    public class ArgumentsEqualMatcherSpecs
    {
        public abstract class Concern : ConcernFor<ArgumentsEqualMatcher>
        {
            protected ICall _call;
            protected ICallSpecification _callSpecification;
            protected bool _result;

            public override void Because()
            {
                _result = sut.IsMatch(_call, _callSpecification);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _callSpecification = mock<ICallSpecification>();
            }

            public override ArgumentsEqualMatcher CreateSubjectUnderTest()
            {
                return new ArgumentsEqualMatcher(mock<IArgumentEqualityComparer>());
            }

            protected void SetupCallArguments(ICall call, object[] arguments)
            {
                call.stub(x => x.GetArguments()).Return(arguments);
            }

            protected void SetupArgumentMatchers(ICallSpecification specification, params bool[] matches)
            {
                var argumentMatchers = new List<IArgumentMatcher>();
                foreach (var match in matches)
                {
                    var argumentMatcher = MockRepository.GenerateStub<IArgumentMatcher>();
                    argumentMatcher.Stub(x => x.Matches(Arg<object>.Is.Anything)).Return(match);
                    argumentMatchers.Add(argumentMatcher);
                }
                specification.Stub(x => x.ArgumentMatchers).Return(argumentMatchers);
            }
        }

        public class When_calls_have_the_same_number_of_arguments_and_they_all_are_equal : Concern
        {
            public override void Context()
            {
                base.Context();
                SetupCallArguments(_call, new object[2]);
                SetupArgumentMatchers(_callSpecification, true, true);
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
                SetupCallArguments(_call, new object[2]);
                SetupArgumentMatchers(_callSpecification, true, true, true);
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
                SetupCallArguments(_call, new object[2]);
                SetupArgumentMatchers(_callSpecification, true, false);
            }

            [Test]
            public void Should_not_match()
            {
                Assert.That(_result, Is.False);
            }
        }

    }
}