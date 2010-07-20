using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class EventRaising
    {
        [Test]
        public void Raise_event_with_standard_event_args()
        {
            var sender = new object();
            var arguments = new EventArgs();

            var engine = Substitute.For<IEngine>();
            var stoppedHandler = new RaisedEventRecorder<EventArgs>();
            engine.Stopped += stoppedHandler.Record;
            engine.Stopped += Raise.Event(sender, arguments);

            Assert.That(stoppedHandler.Sender, Is.SameAs(sender));
            Assert.That(stoppedHandler.EventArgs, Is.SameAs(arguments));
        }

        [Test]
        public void Raise_event_with_custom_event_args()
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

        [Test]
        public void Raise_event_with_standard_event_args_and_sender_automatically_set_to_substitute()
        {
            var arguments = new EventArgs();

            var engine = Substitute.For<IEngine>();
            var stoppedEventHandler = new RaisedEventRecorder<EventArgs>();
            engine.Stopped += stoppedEventHandler.Record;
            engine.Stopped += Raise.Event(arguments);

            Assert.That(stoppedEventHandler.Sender, Is.SameAs(engine));
            Assert.That(stoppedEventHandler.EventArgs, Is.SameAs(arguments));
        }

        [Test]
        public void Raise_event_with_standard_event_args_as_generic_and_sender_automatically_set_to_substitute()
        {
            var arguments = new EventArgs();

            var engine = Substitute.For<IEngine>();
            var brokenEventHandler = new RaisedEventRecorder<EventArgs>();
            engine.Broken += brokenEventHandler.Record;
            engine.Broken += Raise.Event(arguments);

            Assert.That(brokenEventHandler.Sender, Is.SameAs(engine));
            Assert.That(brokenEventHandler.EventArgs, Is.SameAs(arguments));
        }

        [Test]
        public void Raise_event_with_custom_event_args_and_sender_automatically_set_to_substitute()
        {
            var arguments = new IdlingEventArgs();

            var engine = Substitute.For<IEngine>();
            var idlingHandler = new RaisedEventRecorder<IdlingEventArgs>();
            engine.Idling += idlingHandler.Record;
            engine.Idling += Raise.Event(arguments);

            Assert.That(idlingHandler.Sender, Is.SameAs(engine));
            Assert.That(idlingHandler.EventArgs, Is.SameAs(arguments));
        }

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
        public void Raise_event_with_custom_event_args_and_automatically_set_sender_and_args()
        {
            var engine = Substitute.For<IEngine>();
            var idlingHandler = new RaisedEventRecorder<IdlingEventArgs>();
            engine.Idling += idlingHandler.Record;
            engine.Idling += Raise.Event<IdlingEventArgs>();

            Assert.That(idlingHandler.Sender, Is.SameAs(engine));
            Assert.That(idlingHandler.EventArgs, Is.Null);
            
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

        [Test]
        public void Raise_event_declared_as_action_with_one_parameter()
        {
            var rpm = 0;
            var engine = Substitute.For<IEngine>();
            engine.RevvedAt += x => rpm = x;

            Assert.That(rpm, Is.EqualTo(0));
            engine.RevvedAt += Raise.Action(42);
            Assert.That(rpm, Is.EqualTo(42));
        }

        [Test]
        public void Raise_event_declared_as_action_with_two_parameters()
        {
            var initialPercent = 0;
            var finalPercent = 0;
            var engine = Substitute.For<IEngine>();
            engine.PetrolTankFilled += (x, y) =>
                                           {
                                               initialPercent = x;
                                               finalPercent = y;
                                           };


            Assert.That(initialPercent, Is.EqualTo(0));
            Assert.That(finalPercent, Is.EqualTo(0));
            engine.PetrolTankFilled += Raise.Action(20, 80);
            Assert.That(initialPercent, Is.EqualTo(20));
            Assert.That(finalPercent, Is.EqualTo(80));
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