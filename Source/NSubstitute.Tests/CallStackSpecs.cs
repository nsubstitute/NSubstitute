using System;
using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class CallStackSpecs
    {
        public class Concern : ConcernFor<CallStack>
        {
            public override CallStack CreateSubjectUnderTest()
            {
                return new CallStack();
            }
        }

        public class When_a_call_has_been_pushed : Concern
        {
            IInvocation invocation;

            [Test]
            public void Should_pop_to_get_that_call_back()
            {
                Assert.That(sut.Pop(), Is.SameAs(invocation));   
            }

            public override void Because()
            {
                base.Because();
                sut.Push(invocation);
            }

            public override void Context()
            {
                base.Context();
                invocation = mock<IInvocation>();
            }
        }

        public class When_the_call_stack_is_empty : Concern
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