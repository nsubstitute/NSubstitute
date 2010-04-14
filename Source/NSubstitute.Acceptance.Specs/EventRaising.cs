using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [Pending]
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
            engine.Idling += Raise.EventWith(sender, arguments);

            Assert.That(idlingHandler.Sender, Is.SameAs(sender));
            Assert.That(idlingHandler.EventArgs, Is.SameAs(arguments));            
        }

        [Test]
        public void Raise_event_with_sensible_default_arguments()
        {
            var engine = Substitute.For<IEngine>();
            var idlingHandler = new RaisedEventRecorder<IdlingEventArgs>();
            engine.Idling += idlingHandler.Record;
            engine.Idling += Raise.Event;

            Assert.That(idlingHandler.Sender, Is.SameAs(this));
            Assert.That(idlingHandler.EventArgs, Is.Not.Null);
        }

        [Test]
        public void Raise_event_declared_as_action()
        {
            var wasStarted = false;
            var engine = Substitute.For<IEngine>();
            engine.Started += () => wasStarted = true;
            
            Assert.That(wasStarted, Is.False, "Why is this started before event was raised? Something has gone wrong!");            
            engine.Started += Raise.Event;
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

    public static class Raise
    {
        public static EventHandler<TEventArgs> EventWith<TEventArgs>(object sender, TEventArgs arguments) where TEventArgs:EventArgs
        {
            throw new NotImplementedException();
        }

        public static EventHandler<TEventArgs> EventWith<TEventArgs>(TEventArgs arguments) where TEventArgs:EventArgs
        {
            throw new NotImplementedException();
        }

        public static EventHandler EventWith(object sender, EventArgs arguments)
        {
            throw new NotImplementedException();
        }

        public static EventHandler EventWith(EventArgs arguments)
        {
            throw new NotImplementedException();
        }

        public static void Event<TEventArgs>(object sender, TEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static void Event()
        {
            throw new NotImplementedException();
        }
    }
}