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

        [Test]
        public void RaiseEventHandlerEvent()
        {
            var eventSource = Substitute.For<IEventSource>();
            var consumer = new SampleEventConsumer(eventSource);

            eventSource.AnEventHandlerEvent += Raise.Event();
            Assert.That(consumer.EventHandled, Is.True, "Expected event to be raised and handled by the SUT.");
        }

        [Test]
        public void RaiseEventHandlerEventWithArgType()
        {
            var eventSource = Substitute.For<IEventSource>();
            var consumer = new SampleEventConsumer(eventSource);

            eventSource.AnEventHandlerEventWithArgType += Raise.Event();
            Assert.That(consumer.EventHandled, Is.True, "Expected event to be raised and handled by the SUT.");
        }
        
        public interface IEventSource { 
            event Action AnEvent;
            event EventHandler<EventArgs> AnEventHandlerEventWithArgType;
            event EventHandler AnEventHandlerEvent;
        }

        public class SampleEventConsumer
        {
            public bool EventHandled { get; set; }

            public SampleEventConsumer(IEventSource eventSource)
            {
                eventSource.AnEvent += () => EventHandled = true;
                eventSource.AnEventHandlerEvent += (sender, args) => EventHandled = true;
                eventSource.AnEventHandlerEventWithArgType += (sender, args) => EventHandled = true;
            }
        }
    }
}