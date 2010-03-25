using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallSpecificationFactorySpec
    {
        public abstract class Concern : ConcernFor<CallSpecificationFactory>
        {
            protected ICall _call;
            protected object[] _callArguments;
            protected IList<IArgumentMatcher> _argumentMatchers;

            public override void Context()
            {
                base.Context();
                _argumentMatchers = new List<IArgumentMatcher>();
                _callArguments = new object[] { 1, "fred" };
                _call = mock<ICall>();
                _call.stub(x => x.GetMethodInfo()).Return(mock<MethodInfo>());
                _call.stub(x => x.GetArguments()).Return(_callArguments);
            }

            public override CallSpecificationFactory CreateSubjectUnderTest()
            {
                return new CallSpecificationFactory();
            }
        }

        public abstract class When_creating_a_specification_successfully : Concern
        {
            protected ICallSpecification _result;

            [Test]
            public void Should_set_methodInfo_on_result()
            {
                Assert.That(_result.MethodInfo, Is.SameAs(_call.GetMethodInfo()));
            }

            public override void Because()
            {
                _result = sut.Create(_call, _argumentMatchers);
            }
        }

        public class When_creating_a_specification_with_no_argument_matchers : When_creating_a_specification_successfully
        {
            [Test]
            public void Should_set_first_argument_matcher_on_result()
            {
                Assert.That(_result.ArgumentMatchers[0].Matches(_callArguments[0]), Is.True);
            }

            [Test]
            public void Should_set_second_argument_matcher_on_result()
            {
                Assert.That(_result.ArgumentMatchers[1].Matches(_callArguments[1]), Is.True);
            }
        }

        public class When_creating_a_specification_with_correct_number_of_argument_matchers : When_creating_a_specification_successfully
        {
            [Test]
            public void Should_set_first_argument_matcher_on_result()
            {
                Assert.That(_result.ArgumentMatchers[0], Is.EqualTo(_argumentMatchers[0]));
            }

            [Test]
            public void Should_set_second_argument_matcher_on_result()
            {
                Assert.That(_result.ArgumentMatchers[1], Is.EqualTo(_argumentMatchers[1]));
            }

            public override void Context()
            {
                base.Context();
                _argumentMatchers.Add(mock<IArgumentMatcher>());
                _argumentMatchers.Add(mock<IArgumentMatcher>());
            }
        }

        public class When_creating_a_specification_with_incorrect_number_of_argument_matchers : Concern
        {
            [Test]
            public void Should_throw_exception_on_create()
            {
                Assert.Throws<AmbiguousParametersException>(() => sut.Create(_call, _argumentMatchers));
            }

             public override void Context()
            {
                base.Context();
                _argumentMatchers.Add(mock<IArgumentMatcher>());
            }
        }
    }
}