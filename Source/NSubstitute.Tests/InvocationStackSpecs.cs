using System;
using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class InvocationStackSpecs
    {
        public abstract class Concern : ConcernFor<InvocationStack>
        {
            public override InvocationStack CreateSubjectUnderTest()
            {
                return new InvocationStack();
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
                invocation = mock<IInvocation>();
            }
        }

        public class When_the_invocation_stack_is_empty : Concern
        {
            [Test]
            public void Should_get_a_stack_empty_exception()
            {
                var exception = Assert.Throws<InvalidOperationException>(() => sut.Pop());
                Assert.That(exception.Message, Text.Contains("Stack empty"));
            }
        }
    }
}