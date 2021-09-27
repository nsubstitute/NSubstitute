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

        public interface IEngine
        {
            event Action Started;
        }
    }
}