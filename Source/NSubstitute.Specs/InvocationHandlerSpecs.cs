using System;
using NSubstitute.Specs.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class InvocationHandlerSpecs
    {
        public abstract class Concern : ConcernFor<InvocationHandler>
        {
            protected int valueToReturn;
            protected ISubstitutionContext context;
            protected IInvocation invocation;
            protected IInvocationStack invocationStack;
            protected IInvocationResults configuredResults;

            public override void Context()
            {
                valueToReturn = 7;
                context = mock<ISubstitutionContext>();
                invocationStack = mock<IInvocationStack>();
                configuredResults = mock<IInvocationResults>();
                invocation = mock<IInvocation>();
            }

            public override InvocationHandler CreateSubjectUnderTest()
            {
                return new InvocationHandler(invocationStack, configuredResults, context);
            } 
        }

        public class When_a_member_is_invoked : Concern
        {
            object result;

            [Test]
            public void Should_record_invocation()
            {
                invocationStack.received(x => x.Push(invocation));
            }

            [Test]
            public void Should_update_last_invocation_handler_on_substitution_context()
            {
                context.received(x => x.LastInvocationHandlerInvoked(sut));
            }

            [Test]
            public void Should_return_value_from_configured_results()
            {
                Assert.That(result, Is.EqualTo(valueToReturn));
            }

            public override void Because()
            {
                result = sut.HandleInvocation(invocation);
            }

            public override void Context()
            {
                base.Context();
                configuredResults.stub(x => x.GetResult(invocation)).Return(valueToReturn);
            }
        }

        public class When_the_return_value_for_the_last_invocation_is_set : Concern
        {            
            [Test]
            public void Should_remove_the_invocation_from_those_recorded_and_add_it_to_configured_results()
            {
                configuredResults.received(x => x.SetResult(invocation, valueToReturn));
            }

            public override void Because()
            {
                sut.LastInvocationShouldReturn(valueToReturn);
            }

            public override void Context()
            {
                base.Context();
                invocationStack.stub(x => x.Pop()).Return(invocation);
            }
        }

        public class When_told_to_assert_the_next_invocation_has_been_received : Concern
        {
            object result;
            object defaultForCall;

            [Test]
            public void Should_throw_exception_if_invocation_has_not_been_received()
            {
                invocationStack.received(x => x.ThrowIfCallNotFound(invocation));                
            }

            [Test]
            public void Should_not_add_invocation_to_stack()
            {
                invocationStack.did_not_receive(x => x.Push(invocation));
            }

            [Test]
            public void Should_return_default_for_invocation()
            {
                Assert.That(result, Is.EqualTo(defaultForCall));
            }

            [Test]
            public void Next_call_should_go_on_stack()
            {
                sut.HandleInvocation(invocation);
                invocationStack.received(x => x.Push(invocation));
            }

            public override void Because()
            {
                sut.AssertNextCallHasBeenReceived();
                result = sut.HandleInvocation(invocation);
            }

            public override void Context()
            {
                base.Context();
                defaultForCall = new object();
                configuredResults.stub(x => x.GetDefaultResultFor(invocation)).Return(defaultForCall);
            }
        }
    }
}