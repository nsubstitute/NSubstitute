using System;
using NSubstitute.Acceptance.Specs.Infrastructure;
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
            Assert.Throws<CallNotReceivedException>(() => engine.Received().Started += someOtherHandler);
        }
    }
}