using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallStackSpecs
    {
        public abstract class Concern : ConcernFor<CallStack>
        {
            public override CallStack CreateSubjectUnderTest()
            {
                return new CallStack();
            }
        }

        public class When_a_call_has_been_pushed : Concern
        {
            ICall _call;

            [Test]
            public void Should_pop_to_get_that_call_back()
            {
                Assert.That(sut.Pop(), Is.SameAs(_call));   
            }

            public override void Because()
            {
                sut.Push(_call);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
            }
        }

        public class When_the_call_stack_is_empty : Concern
        {
            [Test]
            public void Should_get_a_stack_empty_exception_when_trying_to_pop()
            {
                var exception = Assert.Throws<InvalidOperationException>(() => sut.Pop());
                Assert.That(exception.Message, Text.Contains("Stack empty"));
            }

            [Test]
            public void Should_throw_when_checking_if_a_call_is_found()
            {
                Assert.Throws<CallNotReceivedException>(() => sut.ThrowIfCallNotFound(mock<ICallSpecification>()));
            }
        }

        public class When_checking_multiple_calls_received_and_checking_for_a_specific_call : Concern
        {
            private ICallSpecification _callSpecificationToCheck;
            private ICall _firstCall;
            private ICall _secondCall;

            [Test]
            public void Should_throw_if_no_matches_for_call_are_found()
            {
                Assert.Throws<CallNotReceivedException>(() => sut.ThrowIfCallNotFound(mock<ICallSpecification>()));                
            }

            [Test]
            public void Should_not_throw_is_a_matching_call_is_found()
            {
                sut.ThrowIfCallNotFound(_callSpecificationToCheck);
                Assert.Pass();
            }

            public override void Because()
            {
                _callSpecificationToCheck.stub(x => x.IsSatisfiedBy(_secondCall)).Return(true);
                sut.Push(_firstCall);
                sut.Push(_secondCall);
            }

            public override void Context()
            {
                base.Context();
                _firstCall = mock<ICall>();
                _secondCall = mock<ICall>();
                _callSpecificationToCheck = mock<ICallSpecification>();
            }
        }

        public class When_finding_calls_matching_a_specification : Concern
        {
            private ICallSpecification _callSpecificationToCheck;
            private ICall _callThatDoesNotMatchSpec;
            private ICall _callThatMatchesSpec;
            private ICall _anotherCallThatMatchesSpec;
            private IEnumerable<ICall> _result;

            [Test]
            public void Should_return_all_calls_that_satisfy_specification()
            {
                Assert.That(_result, Is.EquivalentTo(new[] { _callThatMatchesSpec, _anotherCallThatMatchesSpec })); 
            }

            public override void Because()
            {
                sut.Push(_callThatMatchesSpec);
                sut.Push(_callThatDoesNotMatchSpec);
                sut.Push(_anotherCallThatMatchesSpec);
                _result = sut.FindMatchingCalls(_callSpecificationToCheck);
            }

            public override void Context()
            {
                base.Context();
                _callThatDoesNotMatchSpec = mock<ICall>();
                _callThatMatchesSpec = mock<ICall>();
                _anotherCallThatMatchesSpec = mock<ICall>();
                _callSpecificationToCheck = mock<ICallSpecification>();
                _callSpecificationToCheck.stub(x => x.IsSatisfiedBy(_callThatMatchesSpec)).Return(true);
                _callSpecificationToCheck.stub(x => x.IsSatisfiedBy(_anotherCallThatMatchesSpec)).Return(true);
            }
        }
    }
}