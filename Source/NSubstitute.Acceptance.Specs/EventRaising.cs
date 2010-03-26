using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [Pending]
    public class EventRaising
    {
        [Test]
        public void RaiseEvent()
        {
            var sender = new object();
            var arguments = new EventArgs();

            var engine = Substitute.For<IEngine>();
            var idlingHandler = new RaisedEventRecorder<IdlingEventArgs>();
            engine.Idling += idlingHandler.Record;
            engine.Raise(x => x.Revved += null, sender, arguments);

            Assert.That(idlingHandler.Sender, Is.SameAs(sender));
            Assert.That(idlingHandler.EventArgs, Is.SameAs(arguments));            
        }

        [Test]
        public void RaiseEventWithSensibleDefaultArguments()
        {
            var engine = Substitute.For<IEngine>();
            var idlingHandler = new RaisedEventRecorder<IdlingEventArgs>();
            engine.Idling += idlingHandler.Record;
            engine.Raise(x => x.Revved += null);

            Assert.That(idlingHandler.Sender, Is.SameAs(this));
            Assert.That(idlingHandler.EventArgs, Is.Not.Null);
        }

        [Test]
        public void RaiseEventDeclaredAsAction()
        {
            var wasStarted = false;
            var engine = Substitute.For<IEngine>();
            engine.Started += () => wasStarted = true;
            
            Assert.That(wasStarted, Is.False, "Why is this started before event was raised? Something has gone wrong!");            
            engine.Raise(x => x.Started += null);            
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