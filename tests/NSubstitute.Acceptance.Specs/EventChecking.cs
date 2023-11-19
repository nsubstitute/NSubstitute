using System;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class EventChecking
    {
        [Test]
        public void Check_if_event_was_subscribed_to()
        {
            var engine = Substitute.For<IEngine>();
            Action handler = () => { };
            Action someOtherHandler = () => { };
            engine.Started += handler;
            engine.Received().Started += handler;
            Assert.Throws<ReceivedCallsException>(() => engine.Received().Started += someOtherHandler);
        }

        [Test]
        public void Check_if_nullHandlers_are_ignored()
        {
            var raised = false;
            var source = Substitute.For<IEngine>();
            source.Started += null;
            source.Started += () => raised = true;
            source.Started += Raise.Event<Action>();

            Assert.IsTrue(raised);
        }

        [Test]
        public void Check_if_multiple_handlers_get_called()
        {
            var raised1 = false;
            var raised2 = false;
            var raised3 = false;
            var source = Substitute.For<IEngine>();
            source.Started += () => raised1 = true;
            source.Started += () => raised2 = true;
            source.Started += () => raised3 = true;
            source.Started += Raise.Event<Action>();

            Assert.IsTrue(raised1, "The first handler was not called");
            Assert.IsTrue(raised2, "The second handler was not called");
            Assert.IsTrue(raised3, "The third handler was not called");
        }

        public interface IEngine
        {
            event Action Started;
        }
    }
}