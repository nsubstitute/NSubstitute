using System;
using NSubstitute.Core;
using NSubstitute.ReturnsExtensions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class GetCallSpecSpecs
    {
        public abstract class Concern : ConcernFor<GetCallSpec>
        {
            protected ICallCollection _callCollection;
            protected IPendingSpecification _pendingSpecification;
            protected ICallActions _callActions;
            protected ICallSpecificationFactory _callSpecificationFactory;

            public override void Context()
            {
                _callCollection = mock<ICallCollection>();
                _pendingSpecification = mock<IPendingSpecification>();
                _callActions = mock<ICallActions>();
                _callSpecificationFactory = mock<ICallSpecificationFactory>();
            }

            public override GetCallSpec CreateSubjectUnderTest()
            {
                return new GetCallSpec(_callCollection, _pendingSpecification, _callSpecificationFactory, _callActions);
            }
        }

        public class When_getting_for_pending_call_spec_with_pending_spec_and_matching_args : Concern
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
            public void Should_not_touch_call_collection()
            {
                _callCollection.did_not_receive(x => x.Delete(It.IsAny<ICall>()));
            }

            public override void Because()
            {
                _result = sut.FromPendingSpecification(MatchArgs.AsSpecifiedInCall);
            }

            public override void Context()
            {
                base.Context();
                _lastCallSpecification = mock<ICallSpecification>();
                _pendingSpecification.stub(x => x.HasPendingCallSpecInfo()).Return(true);

                var specificationInfo = PendingSpecificationInfo.FromCallSpecification(_lastCallSpecification);
                _pendingSpecification.stub(x => x.UseCallSpecInfo()).Return(specificationInfo);
            }
        }

        public class When_getting_for_pending_spec_with_pending_spec_and_setting_for_any_args : Concern
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
                _callCollection.did_not_receive(x => x.Delete(It.IsAny<ICall>()));
            }

            public override void Because()
            {
                _result = sut.FromPendingSpecification(MatchArgs.Any);
            }

            public override void Context()
            {
                base.Context();
                _callSpecUpdatedForAnyArgs = mock<ICallSpecification>();
                _lastCallSpecification = mock<ICallSpecification>();

                var specificationInfo = PendingSpecificationInfo.FromCallSpecification(_lastCallSpecification);

                _pendingSpecification.stub(x => x.HasPendingCallSpecInfo()).Return(true);
                _pendingSpecification.stub(x => x.UseCallSpecInfo()).Return(specificationInfo);
                _lastCallSpecification.stub(x => x.CreateCopyThatMatchesAnyArguments()).Return(_callSpecUpdatedForAnyArgs);
            }
        }

        public class When_getting_for_pending_spec_with_last_call_info : Concern
        {
            readonly MatchArgs _argMatchStrategy = MatchArgs.AsSpecifiedInCall;
            ICall _call;
            ICallSpecification _callSpecification;
            ICallSpecification _result;

            [Test]
            public void Should_take_last_call_from_pending_specification_info()
            {
                Assert.That(_result, Is.EqualTo(_callSpecification));
            }

            [Test]
            public void Should_delete_last_call_from_call_collection()
            {
                _callCollection.received(c => c.Delete(_call));
            }

            public override void Because()
            {
                _result = sut.FromPendingSpecification(_argMatchStrategy);
            }

            public override void Context()
            {
                base.Context();

                _call = mock<ICall>();

                var specificationInfo = PendingSpecificationInfo.FromLastCall(_call);

                _pendingSpecification.stub(x => x.HasPendingCallSpecInfo()).Return(true);
                _pendingSpecification.stub(x => x.UseCallSpecInfo()).Return(specificationInfo);

                _callSpecification = mock<ICallSpecification>();
                _callSpecificationFactory.stub(x => x.CreateFrom(_call, _argMatchStrategy)).Return(_callSpecification);
            }
        }

        public class When_no_pending_specification : Concern
        {
            [Test]
            public void Should_fail_with_exception()
            {
                Assert.That(() => sut.FromPendingSpecification(MatchArgs.AsSpecifiedInCall), Throws.InvalidOperationException);
            }

            public override void Context()
            {
                base.Context();

                _pendingSpecification.stub(s => s.HasPendingCallSpecInfo()).Return(false);
            }
        }
    }
}