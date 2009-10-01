using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class InvocationHandlerSpecs
    {
        public abstract class Concern : ConcernFor<InvocationHandler>
        {
            public int valueToReturn;
            public ISubstitutionContext context;
            public IInvocation invocation;
            public IInvocationStack InvocationStack;
            public IInvocationResults configuredResults;

            public override void Context()
            {
                valueToReturn = 7;
                context = mock<ISubstitutionContext>();
                InvocationStack = mock<IInvocationStack>();
                configuredResults = mock<IInvocationResults>();
                invocation = mock<IInvocation>();
            }

            public override InvocationHandler CreateSubjectUnderTest()
            {
                return new InvocationHandler(InvocationStack, configuredResults, context);
            } 
        }

        public class When_a_member_is_invoked : Concern
        {
            object result;

            [Test]
            public void Should_record_invocation()
            {
                InvocationStack.received(x => x.Push(invocation));
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
                InvocationStack.stub(x => x.Pop()).Return(invocation);
            }
        }
    }
}