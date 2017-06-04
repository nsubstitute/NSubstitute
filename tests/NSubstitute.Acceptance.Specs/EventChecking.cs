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

        public interface IEngine
        {
            event Action Started;
        }
    }
}