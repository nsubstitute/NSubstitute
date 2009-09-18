using NSubstitute.Tests.TestInfrastructure;
using NUnit.Framework;

namespace NSubstitute.Tests
{
    public class SubstituteSpecs
    {
        public abstract class Concern : ConcernFor<Substitute>
        {
            public int valueToReturn;
            public ISubstitutionContext context;
            public IInvocation invocation;
            public ICallStack callStack;
            public ICallResults configuredResults;

            public override void Context()
            {
                valueToReturn = 7;
                context = mock<ISubstitutionContext>();
                callStack = mock<ICallStack>();
                configuredResults = mock<ICallResults>();
                invocation = mock<IInvocation>();
            }

            public override Substitute CreateSubjectUnderTest()
            {
                return new Substitute(callStack, configuredResults, context);
            } 
        }

        public class When_a_member_is_invoked : Concern
        {
            [Test]
            public void Should_record_invocation()
            {
                callStack.received(x => x.Push(invocation));
            }

            [Test]
            public void Should_update_last_called_substitute_on_substitution_context()
            {
                context.received(x => x.LastSubstituteCalled(sut));
            }

            [Test]
            public void Should_set_value_to_return_from_configured_results()
            {
                invocation.received(x => x.SetReturn(valueToReturn));
            }

            public override void Because()
            {
                sut.MemberInvoked(invocation);
            }

            public override void Context()
            {
                base.Context();
                configuredResults.stub(x => x.GetResult(invocation)).Return(valueToReturn);
            }
        }

        public class When_the_return_value_for_the_last_call_is_set : Concern
        {
            
            [Test]
            public void Should_remove_the_invocation_from_the_recorded_calls_and_add_it_to_configured_results()
            {
                configuredResults.received(x => x.SetResult(invocation, valueToReturn));
            }

            public override void Because()
            {
                sut.LastCallShouldReturn(valueToReturn);
            }

            public override void Context()
            {
                base.Context();
                callStack.stub(x => x.Pop()).Return(invocation);
            }
        }
    }
}