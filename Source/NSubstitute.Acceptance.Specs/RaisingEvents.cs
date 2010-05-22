using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class RaisingEvents
    {
        [Test]
        public void RaiseActionEvent()
        {
            var eventSource = Substitute.For<IEventSource>();
            var consumer = new SampleEventConsumer(eventSource);

            eventSource.AnEvent += Raise.Action();
            Assert.That(consumer.EventHandled, "Expected event to be raised and handled by the SUT.");
        }


        public interface IEventSource { event Action AnEvent; }

        public class SampleEventConsumer
        {
            public bool EventHandled { get; set; }

            public SampleEventConsumer(IEventSource eventSource)
            {
                eventSource.AnEvent += () => EventHandled = true;
            }
        }
    }
}