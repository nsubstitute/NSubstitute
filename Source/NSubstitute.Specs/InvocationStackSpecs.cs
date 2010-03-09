using System;
using NSubstitute.Specs.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class InvocationStackSpecs
    {
        public abstract class Concern : ConcernFor<InvocationStack>
        {
            protected IInvocationMatcher invocationMatcher;

            public override void Context()
            {
                invocationMatcher = mock<IInvocationMatcher>();
            }

            public override InvocationStack CreateSubjectUnderTest()
            {
                return new InvocationStack(invocationMatcher);
            }
        }

        public class When_an_invocation_has_been_pushed : Concern
        {
            IInvocation invocation;

            [Test]
            public void Should_pop_to_get_that_call_back()
            {
                Assert.That(sut.Pop(), Is.SameAs(invocation));   
            }

            public override void Because()
            {
                sut.Push(invocation);
            }

            public override void Context()
            {
                base.Context();
                invocation = mock<IInvocation>();
            }
        }

        public class When_the_invocation_stack_is_empty : Concern
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
                Assert.Throws<CallNotReceivedException>(() => sut.ThrowIfCallNotFound(mock<IInvocation>()));
            }
        }

        public class When_checking_multiple_invocations_received_and_checking_for_a_specific_invocation : Concern
        {
            private IInvocation invocationToCheck;
            private IInvocation firstInvocation;
            private IInvocation secondInvocation;

            [Test]
            public void Should_throw_if_no_matches_for_invocation_are_found()
            {
                Assert.Throws<CallNotReceivedException>(() => sut.ThrowIfCallNotFound(mock<IInvocation>()));                
            }

            [Test]
            public void Should_not_throw_is_a_matchinfginvocation_is_found()
            {
                sut.ThrowIfCallNotFound(invocationToCheck);
                Assert.Pass();
            }

            public override void Because()
            {
                invocationMatcher.stub(x => x.IsMatch(secondInvocation, invocationToCheck)).Return(true);
                sut.Push(firstInvocation);
                sut.Push(secondInvocation);
            }

            public override void Context()
            {
                base.Context();
                firstInvocation = mock<IInvocation>();
                secondInvocation = mock<IInvocation>();
                invocationToCheck = mock<IInvocation>();
            }
        }
    }
}