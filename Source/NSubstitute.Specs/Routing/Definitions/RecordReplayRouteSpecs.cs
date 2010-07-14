using NSubstitute.Core;
using NSubstitute.Routing.Definitions;
using NSubstitute.Routing.Handlers;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Definitions
{
    public class RecordReplayRouteSpecs : ConcernForRoute<RecordReplayRoute>
    {
        [Test]
        public void Should_set_properties()
        {
            AssertPartHandledCall<PropertySetterHandler>();
        }

        [Test]
        public void Should_handle_event_subscriptions()
        {
            AssertPartHandledCall<EventSubscriptionHandler>();
        }

        [Test]
        public void Should_record_call()
        {
            AssertPartHandledCall<RecordCallHandler>();
        }

        [Test]
        public void Should_do_matching_call_actions()
        {
            AssertPartHandledCall<DoActionsCallHandler>();
        }

        public override void Context()
        {
            base.Context();
            ExpectReturnValueToComeFromPart<ReturnConfiguredResultHandler>();
        }
    }
}