using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class GetCallSpecSpecs
    {
        public abstract class Concern : ConcernFor<GetCallSpec>
        {
            protected ICallStack _callStack;
            protected IPendingSpecification _pendingSpecification;
            protected ICallActions _callActions;
            protected ICallSpecificationFactory _callSpecificationFactory;

            public override void Context()
            {
                _callStack = mock<ICallStack>();
                _pendingSpecification = mock<IPendingSpecification>();
                _callActions = mock<ICallActions>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
            }

            public override GetCallSpec CreateSubjectUnderTest()
            {
                return new GetCallSpec(_callStack, _pendingSpecification, _callSpecificationFactory, _callActions);
            }
        }

        public class When_getting_for_last_call_with_pending_spec_and_matching_args : Concern
        {
            ICallSpecification _lastCallSpecification;
            ICallSpecification _result;

            [Test]
            public void Should_use_that_spec()
            {
                Assert.That(_result, Is.EqualTo(_lastCallSpecification));
            }

            [Test]
            public void Should_not_alter_existing_actions_for_last_call_specification()
            {
                _callActions.did_not_receive(x => x.MoveActionsForSpecToNewSpec(It.IsAny<ICallSpecification>(), It.IsAny<ICallSpecification>()));
            }

            [Test]
            public void Should_not_touch_call_stack()
            {
                _callStack.did_not_receive(x => x.Pop());
            }

            public override void Because()
            {
                _result = sut.FromLastCall(MatchArgs.AsSpecifiedInCall);
            }

            public override void Context()
            {
                base.Context();
                _lastCallSpecification = mock<ICallSpecification>();
                _pendingSpecification.stub(x => x.HasPendingCallSpec()).Return(true);
                _pendingSpecification.stub(x => x.UseCallSpec()).Return(_lastCallSpecification);
            }
        }

        public class When_getting_for_last_call_with_pending_spec_and_setting_for_any_args : Concern
        {
            ICallSpecification _callSpecUpdatedForAnyArgs;
            ICallSpecification _lastCallSpecification;
            private ICallSpecification _result;

            [Test]
            public void Should_use_pending_call_spec_updated_to_match_any_args()
            {
                Assert.That(_result, Is.EqualTo(_callSpecUpdatedForAnyArgs));
            }

            [Test]
            public void Should_move_actions_from_last_spec_to_spec_for_any_arguments()
            {
                _callActions.received(x => x.MoveActionsForSpecToNewSpec(_lastCallSpecification, _callSpecUpdatedForAnyArgs));
            }

            [Test]
            public void Should_not_touch_call_stack()
            {
                _callStack.did_not_receive(x => x.Pop());
            }

            public override void Because()
            {
                _result = sut.FromLastCall(MatchArgs.Any);
            }

            public override void Context()
            {
                base.Context();
                _callSpecUpdatedForAnyArgs = mock<ICallSpecification>();
                _lastCallSpecification = mock<ICallSpecification>();
                _pendingSpecification.stub(x => x.HasPendingCallSpec()).Return(true);
                _pendingSpecification.stub(x => x.UseCallSpec()).Return(_lastCallSpecification);
                _lastCallSpecification.stub(x => x.CreateCopyThatMatchesAnyArguments()).Return(_callSpecUpdatedForAnyArgs);
            }
        }

        public class When_getting_for_last_call_with_no_pending_call_spec : Concern
        {
            readonly MatchArgs _argMatchStrategy = MatchArgs.AsSpecifiedInCall;
            ICall _call;
            ICallSpecification _callSpecification;
            ICallSpecification _result;

            [Test]
            public void Should_remove_the_call_from_those_recorded_and_add_it_to_configured_results()
            {
                Assert.That(_result, Is.EqualTo(_callSpecification));
            }

            public override void Because()
            {
                _result = sut.FromLastCall(_argMatchStrategy);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
                _callStack.stub(x => x.Pop()).Return(_call);
                _pendingSpecification.stub(x => x.HasPendingCallSpec()).Return(false);

                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _argMatchStrategy)).Return(_callSpecification);
            }
        }
    
    }
}