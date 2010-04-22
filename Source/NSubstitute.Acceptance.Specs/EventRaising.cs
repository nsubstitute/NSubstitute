using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class EventRaising
    {
        [Test]
        public void Raise_event()
        {
            var sender = new object();
            var arguments = new IdlingEventArgs();

            var engine = Substitute.For<IEngine>();
            var idlingHandler = new RaisedEventRecorder<IdlingEventArgs>();
            engine.Idling += idlingHandler.Record;
            engine.Idling += Raise.Event(sender, arguments);

            Assert.That(idlingHandler.Sender, Is.SameAs(sender));
            Assert.That(idlingHandler.EventArgs, Is.SameAs(arguments));
        }

        [Pending]
        [Test]
        public void Raise_event_with_sensible_default_arguments()
        {
            var engine = Substitute.For<IEngine>();
            var stoppedHandler = new RaisedEventRecorder<EventArgs>();
            engine.Stopped += stoppedHandler.Record;
            engine.Stopped += Raise.Event();

            Assert.That(stoppedHandler.Sender, Is.EqualTo(engine));
            Assert.That(stoppedHandler.EventArgs, Is.EqualTo(EventArgs.Empty));
        }

        [Test]
        public void Raise_event_declared_as_action()
        {
            var wasStarted = false;
            var engine = Substitute.For<IEngine>();
            engine.Started += () => wasStarted = true;

            Assert.That(wasStarted, Is.False, "Why is this started before event was raised? Something has gone wrong!");
            engine.Started += Raise.Action();
            Assert.That(wasStarted);
        }

        class RaisedEventRecorder<T>
        {
            public object Sender;
            public T EventArgs;

            public void Record(object sender, T eventArgs)
            {
                Sender = sender;
                EventArgs = eventArgs;
            }
        }
    }
}