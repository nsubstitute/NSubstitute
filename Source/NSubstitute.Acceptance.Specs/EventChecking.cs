using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class EventChecking
    {
        [Test]
        public void CheckIfEventWasSubscribedTo()
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