using System;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NSubstitute.Exceptions;
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
            engine.Stopped += Raise.EventWith(sender, arguments);

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
            engine.Idling += Raise.EventWith(sender, arguments);

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
            engine.Stopped += Raise.EventWith(arguments);

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
            engine.Broken += Raise.EventWith(arguments);

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
            engine.Idling += Raise.EventWith(arguments);

            Assert.That(idlingHandler.Sender, Is.SameAs(engine));
            Assert.That(idlingHandler.EventArgs, Is.SameAs(arguments));
        }

        [Test]
        public void Raise_event_with_empty_event_args()
        {
            var engine = Substitute.For<IEngine>();
            var stoppedHandler = new RaisedEventRecorder<EventArgs>();
            engine.Stopped += stoppedHandler.Record;
            engine.Stopped += Raise.EventWithEmptyEventArgs();

            Assert.That(stoppedHandler.Sender, Is.EqualTo(engine));
            Assert.That(stoppedHandler.EventArgs, Is.EqualTo(EventArgs.Empty));
        }

        [Test]
        public void Raise_event_with_custom_event_args_that_have_a_default_ctor_and_automatically_set_sender_and_args()
        {
            var engine = Substitute.For<IEngine>();
            var idlingHandler = new RaisedEventRecorder<IdlingEventArgs>();
            engine.Idling += idlingHandler.Record;
            engine.Idling += Raise.EventWith<IdlingEventArgs>();

            Assert.That(idlingHandler.Sender, Is.SameAs(engine));
            Assert.That(idlingHandler.EventArgs, Is.Not.Null);
        }

        [Test]
        public void Raise_event_with_custom_event_args_with_no_default_ctor_should_throw()
        {
            var engine = Substitute.For<IEngine>();
            var lowFuelHandler = new RaisedEventRecorder<LowFuelWarningEventArgs>();
            engine.LowFuelWarning += lowFuelHandler.Record;

            Assert.Throws<CannotCreateEventArgsException>(() =>
                engine.LowFuelWarning += Raise.EventWith<LowFuelWarningEventArgs>()
                );
        }

        [Test]
        public void Raise_event_declared_as_action_using_shortcut_syntax()
        {
            var wasStarted = false;
            var engine = Substitute.For<IEngine>();
            engine.Started += () => wasStarted = true;

            Assert.That(wasStarted, Is.False, "Why is this started before event was raised? Something has gone wrong!");
            engine.Started += Raise.Action();
            Assert.That(wasStarted);
        }

        [Test]
        public void Raise_event_declared_as_action()
        {
            var wasStarted = false;
            var engine = Substitute.For<IEngine>();
            engine.Started += () => wasStarted = true;

            Assert.That(wasStarted, Is.False, "Why is this started before event was raised? Something has gone wrong!");
            engine.Started += Raise.Event<Action>();
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

        [Test]
        public void Raise_event_declared_as_custom_delegate_type()
        {
            var temperature = 0;
            var engine = Substitute.For<IEngine>();
            engine.Overheating += x => temperature = x;


            Assert.That(temperature, Is.EqualTo(0));
            engine.Overheating += Raise.Event<OverheatingEvent>(87);
            Assert.That(temperature, Is.EqualTo(87));
        }

        [Test]
        public void Raise_event_will_throw_same_exception_as_thrown_in_event_handler()
        {
            var engine = Substitute.For<IEngine>();
            engine.LowFuelWarning += (sender, e) => { throw new Exception(); };
            Assert.Throws<Exception>(() => engine.LowFuelWarning += Raise.EventWith(new LowFuelWarningEventArgs(5)));
        }

        class RaisedEventRecorder<T>
        {
            public object Sender;
            public T EventArgs;
            public bool WasCalled;

            public void Record(object sender, T eventArgs)
            {
                WasCalled = true;
                Sender = sender;
                EventArgs = eventArgs;
            }
        }

        [Test]
        public void Raise_event_declared_as_delegate_with_no_args()
        {
            var sub = Substitute.For<IDelegateEvents>();
            var wasRaised = false;
            sub.DelegateEventWithoutArgs += () => wasRaised = true;
            sub.DelegateEventWithoutArgs += Raise.Event<VoidDelegateWithoutArgs>();
            Assert.That(wasRaised);
        }

        [Test]
        public void Raise_event_declared_as_delegate_with_event_args()
        {
            var sub = Substitute.For<IDelegateEvents>();
            var recorder = new RaisedEventRecorder<EventArgs>();
            sub.DelegateEventWithEventArgs += (sender, args) => recorder.Record(sender, args);
            sub.DelegateEventWithEventArgs += Raise.Event<VoidDelegateWithEventArgs>();
            Assert.That(recorder.WasCalled);
            Assert.That(recorder.EventArgs, Is.EqualTo(EventArgs.Empty));
        }

        [Test]
        public void Raise_event_declared_as_delegate_with_an_integer_arg()
        {
            var sub = Substitute.For<IDelegateEvents>();
            var wasCalledWithArg = -1;
            sub.DelegateEventWithAnArg += x => wasCalledWithArg = x;
            sub.DelegateEventWithAnArg += Raise.Event<VoidDelegateWithAnArg>(123);
            Assert.That(wasCalledWithArg, Is.EqualTo(123));
        }

        [Test]
        public void Raise_event_with_incorrect_args()
        {
            var expectedExceptionMessage = string.Format(
@"Raising event of type {0} requires additional arguments.

Use Raise.Event<{0}>({1}) to raise this event.",
                           typeof(VoidDelegateWithAnArg).Name,
                           typeof(int).Name
                );
            var sub = Substitute.For<IDelegateEvents>();
            sub.DelegateEventWithAnArg += x => { };
            Assert.That(
                () => { sub.DelegateEventWithAnArg += Raise.Event<VoidDelegateWithAnArg>(); },
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo(expectedExceptionMessage)
                );
        }
        public interface IDelegateEvents
        {
            event VoidDelegateWithEventArgs DelegateEventWithEventArgs;
            event VoidDelegateWithoutArgs DelegateEventWithoutArgs;
            event VoidDelegateWithAnArg DelegateEventWithAnArg;
        }
        public delegate void VoidDelegateWithoutArgs();
        public delegate void VoidDelegateWithEventArgs(object sender, EventArgs args);
        public delegate void VoidDelegateWithAnArg(int arg);
    }
}