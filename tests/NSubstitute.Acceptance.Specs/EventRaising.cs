using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class EventRaising
{
    [Test]
    public void Raise_event_with_standard_event_args()
    {
        var sender = new object();
        var arguments = new EventArgs();

        var eventSource = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<EventArgs>();
        eventSource.StandardEventHandler += eventRecorder.Record;
        eventSource.StandardEventHandler += Raise.EventWith(sender, arguments);

        Assert.That(eventRecorder.Sender, Is.SameAs(sender));
        Assert.That(eventRecorder.EventArgs, Is.SameAs(arguments));
    }

    [Test]
    public void Raise_event_with_custom_event_args()
    {
        var sender = new object();
        var arguments = new CustomEventArgs();

        var eventSamples = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgs>();
        eventSamples.EventHandlerWithCustomArgs += eventRecorder.Record;
        eventSamples.EventHandlerWithCustomArgs += Raise.EventWith(sender, arguments);

        Assert.That(eventRecorder.Sender, Is.SameAs(sender));
        Assert.That(eventRecorder.EventArgs, Is.SameAs(arguments));
    }

    [Test]
    public void Raise_event_with_standard_event_args_and_sender_automatically_set_to_substitute()
    {
        var arguments = new EventArgs();

        var eventSource = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<EventArgs>();
        eventSource.StandardEventHandler += eventRecorder.Record;
        eventSource.StandardEventHandler += Raise.EventWith(arguments);

        Assert.That(eventRecorder.Sender, Is.SameAs(eventSource));
        Assert.That(eventRecorder.EventArgs, Is.SameAs(arguments));
    }

    [Test]
    public void Raise_event_with_standard_event_args_as_generic_and_sender_automatically_set_to_substitute()
    {
        var arguments = new EventArgs();

        var eventSource = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<EventArgs>();
        eventSource.StandardGenericEventHandler += eventRecorder.Record;
        eventSource.StandardGenericEventHandler += Raise.EventWith(arguments);

        Assert.That(eventRecorder.Sender, Is.SameAs(eventSource));
        Assert.That(eventRecorder.EventArgs, Is.SameAs(arguments));
    }

    [Test]
    public void Raise_event_with_custom_event_args_and_sender_automatically_set_to_substitute()
    {
        var arguments = new CustomEventArgs();

        var eventSamples = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgs>();
        eventSamples.EventHandlerWithCustomArgs += eventRecorder.Record;
        eventSamples.EventHandlerWithCustomArgs += Raise.EventWith(arguments);

        Assert.That(eventRecorder.Sender, Is.SameAs(eventSamples));
        Assert.That(eventRecorder.EventArgs, Is.SameAs(arguments));
    }

    [Test]
    public void Raise_standard_event_with_empty_event_args()
    {
        var eventSamples = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<EventArgs>();
        eventSamples.StandardEventHandler += eventRecorder.Record;
        eventSamples.StandardEventHandler += Raise.Event();

        Assert.That(eventRecorder.Sender, Is.EqualTo(eventSamples));
        Assert.That(eventRecorder.EventArgs, Is.EqualTo(EventArgs.Empty));
    }

    [Test]
    public void Raise_standard_generic_event_with_empty_event_args()
    {
        var eventSamples = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<EventArgs>();
        eventSamples.StandardGenericEventHandler += eventRecorder.Record;
        eventSamples.StandardGenericEventHandler += Raise.Event();

        Assert.That(eventRecorder.Sender, Is.EqualTo(eventSamples));
        Assert.That(eventRecorder.EventArgs, Is.EqualTo(EventArgs.Empty));
    }

    [Test]
    public void Raise_event_with_custom_event_args_that_have_a_default_ctor_and_automatically_set_sender_and_args()
    {
        var eventSamples = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgs>();
        eventSamples.EventHandlerWithCustomArgs += eventRecorder.Record;
        eventSamples.EventHandlerWithCustomArgs += Raise.EventWith<CustomEventArgs>();

        Assert.That(eventRecorder.Sender, Is.SameAs(eventSamples));
        Assert.That(eventRecorder.EventArgs, Is.Not.Null);
    }

    [Test]
    public void Raise_event_with_custom_event_args_with_no_default_ctor_should_throw()
    {
        var eventSamples = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgsWithNoDefaultCtor>();
        eventSamples.EventHandlerWithCustomArgsAndNoDefaultCtor += eventRecorder.Record;

        Assert.Throws<CannotCreateEventArgsException>(() =>
            eventSamples.EventHandlerWithCustomArgsAndNoDefaultCtor += Raise.EventWith<CustomEventArgsWithNoDefaultCtor>()
            );
    }

    [Test]
    public void Raise_event_declared_as_action()
    {
        var wasStarted = false;
        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionEvent += () => wasStarted = true;

        Assert.That(wasStarted, Is.False, "Why is this started before event was raised? Something has gone wrong!");
        eventSamples.ActionEvent += Raise.Event<Action>();
        Assert.That(wasStarted);
    }

    [Test]
    public void Raise_event_declared_as_action_with_one_parameter()
    {
        var arg = 0;
        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.ActionEventWithOneArg += x => arg = x;

        Assert.That(arg, Is.EqualTo(0));
        eventSamples.ActionEventWithOneArg += Raise.Event<Action<int>>(42);
        Assert.That(arg, Is.EqualTo(42));
    }

    [Test]
    public void Raise_event_declared_as_custom_delegate_type()
    {
        var intArg = 0;
        var stringArg = "";
        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.FuncDelegateWithArgs += (x, y) => { intArg = x; stringArg = y; return 0; };

        eventSamples.FuncDelegateWithArgs += Raise.Event<FuncDelegateWithArgs>(87, "hello");
        Assert.That(intArg, Is.EqualTo(87));
        Assert.That(stringArg, Is.EqualTo("hello"));
    }

    [Test]
    public void Raise_event_will_throw_same_exception_as_thrown_in_event_handler()
    {
        var eventSamples = Substitute.For<IEventSamples>();
        eventSamples.EventHandlerWithCustomArgs += (sender, e) => { throw new Exception(); };
        Assert.Throws<Exception>(() => eventSamples.EventHandlerWithCustomArgs += Raise.EventWith(new CustomEventArgs()));
    }

    [Test]
    public void Raise_event_declared_as_delegate_with_no_args()
    {
        var sub = Substitute.For<IEventSamples>();
        var wasRaised = false;
        sub.DelegateEventWithoutArgs += () => wasRaised = true;
        sub.DelegateEventWithoutArgs += Raise.Event<VoidDelegateWithoutArgs>();
        Assert.That(wasRaised);
    }

    [Test]
    public void Raise_event_declared_as_delegate_with_event_args()
    {
        var sub = Substitute.For<IEventSamples>();
        var recorder = new RaisedEventRecorder<EventArgs>();
        sub.DelegateEventWithEventArgs += (sender, args) => recorder.Record(sender, args);
        sub.DelegateEventWithEventArgs += Raise.Event<VoidDelegateWithEventArgs>();
        Assert.That(recorder.WasCalled);
        Assert.That(recorder.EventArgs, Is.EqualTo(EventArgs.Empty));
    }

    [Test]
    public void Raise_event_declared_as_delegate_with_an_integer_arg()
    {
        var sub = Substitute.For<IEventSamples>();
        var wasCalledWithArg = -1;
        sub.DelegateEventWithAnArg += x => wasCalledWithArg = x;
        sub.DelegateEventWithAnArg += Raise.Event<VoidDelegateWithAnArg>(123);
        Assert.That(wasCalledWithArg, Is.EqualTo(123));
    }

    [Test]
    public void Raise_event_with_incorrect_number_of_args()
    {
        var expectedExceptionMessage = string.Format(
                       "Cannot raise event with the provided arguments. Use Raise.Event<{0}>({1}) to raise this event.",
                       typeof(VoidDelegateWithAnArg).Name,
                       typeof(int).Name
            );
        var sub = Substitute.For<IEventSamples>();
        Assert.That(
            () => { sub.DelegateEventWithAnArg += Raise.Event<VoidDelegateWithAnArg>(); },
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(expectedExceptionMessage)
            );
    }

    [Test]
    public void Raise_event_with_incorrect_arg_types()
    {
        var expectedExceptionMessage = string.Format(
                       "Cannot raise event with the provided arguments. Use Raise.Event<{0}>({1}, {2}) to raise this event.",
                       typeof(VoidDelegateWithMultipleArgs).Name,
                       typeof(int).Name,
                       typeof(string).Name
            );
        var sub = Substitute.For<IEventSamples>();
        Assert.That(
            () => { sub.DelegateEventWithMultipleArgs += Raise.Event<VoidDelegateWithMultipleArgs>("test", "test"); },
            Throws.TypeOf<ArgumentException>().With.Message.EqualTo(expectedExceptionMessage)
            );
    }

    [Test]
    public void Raise_event_for_delegate_with_return_value()
    {
        var sub = Substitute.For<IEventSamples>();
        var wasCalled = false;
        sub.FuncDelegate += () => { wasCalled = true; return 0; };
        sub.FuncDelegate += Raise.Event<FuncDelegateWithoutArgs>();
        Assert.That(wasCalled);
    }

    [Test]
    public void Raise_custom_event_that_has_sender_and_args_but_does_not_inherit_from_EventHandler()
    {
        var sender = new object();
        var eventArgs = new CustomEventArgs();
        var sub = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgs>();
        sub.CustomEventThatDoesNotInheritFromEventHandler += eventRecorder.Record;

        sub.CustomEventThatDoesNotInheritFromEventHandler += Raise.Event<CustomEventThatDoesNotInheritFromEventHandler>(sender, eventArgs);
        Assert.That(eventRecorder.WasCalled);
        Assert.That(eventRecorder.EventArgs, Is.SameAs(eventArgs));
        Assert.That(eventRecorder.Sender, Is.SameAs(sender));
    }

    [Test]
    public void Raise_custom_event_that_has_sender_and_args_but_does_not_inherit_from_EventHandler_without_providing_arguments()
    {
        var sub = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgs>();
        sub.CustomEventThatDoesNotInheritFromEventHandler += eventRecorder.Record;

        sub.CustomEventThatDoesNotInheritFromEventHandler += Raise.Event<CustomEventThatDoesNotInheritFromEventHandler>();
        Assert.That(eventRecorder.WasCalled);
        Assert.That(eventRecorder.EventArgs, Is.Not.Null);
        Assert.That(eventRecorder.Sender, Is.SameAs(sub));
    }

    [Test]
    public void Raise_custom_event_that_has_sender_and_args_but_does_not_inherit_from_EventHandler_when_only_providing_event_args()
    {
        var eventArgs = new CustomEventArgs();
        var sub = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgs>();
        sub.CustomEventThatDoesNotInheritFromEventHandler += eventRecorder.Record;

        sub.CustomEventThatDoesNotInheritFromEventHandler += Raise.Event<CustomEventThatDoesNotInheritFromEventHandler>(eventArgs);
        Assert.That(eventRecorder.WasCalled);
        Assert.That(eventRecorder.EventArgs, Is.SameAs(eventArgs));
        Assert.That(eventRecorder.Sender, Is.SameAs(sub));
    }

    [Test]
    public void Raise_custom_event_that_has_sender_and_args_but_does_not_inherit_from_EventHandler_when_only_providing_sender()
    {
        var sender = new object();
        var sub = Substitute.For<IEventSamples>();
        var eventRecorder = new RaisedEventRecorder<CustomEventArgs>();
        sub.CustomEventThatDoesNotInheritFromEventHandler += eventRecorder.Record;

        sub.CustomEventThatDoesNotInheritFromEventHandler += Raise.Event<CustomEventThatDoesNotInheritFromEventHandler>(sender);
        Assert.That(eventRecorder.WasCalled);
        Assert.That(eventRecorder.EventArgs, Is.Not.Null);
        Assert.That(eventRecorder.Sender, Is.SameAs(sender));
    }

    [Test]
    public void MyEvent_with_CustomEventArgsWithInternalDefaultConstructor_is_raised()
    {
        // Arrange
        var exampleInternalMock = Substitute.For<IExampleInternal>();
        var consumerInternal = new ConsumerInternal(exampleInternalMock);

        // Act
        exampleInternalMock.MyEvent += Raise.EventWith<CustomEventArgsWithInternalDefaultConstructor>(this, null!);

        // Assert
        Assert.That(consumerInternal.SomethingWasDone);
    }

    [Test]
    public void MyEvent_with_CustomEventArgsWithPrivateDefaultConstructor_throws_CannotCreateEventArgsException()
    {
        // Arrange
        var examplePrivateMock = Substitute.For<IExamplePrivate>();

        // Act and Assert
        Assert.Throws<CannotCreateEventArgsException>(() =>
            examplePrivateMock.MyEvent += Raise.EventWith<CustomEventArgsWithPrivateDefaultConstructor>(this, null!));
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

    public interface IEventSamples
    {
        event Action ActionEvent;
        event Action<int> ActionEventWithOneArg;
        event VoidDelegateWithEventArgs DelegateEventWithEventArgs;
        event VoidDelegateWithoutArgs DelegateEventWithoutArgs;
        event VoidDelegateWithAnArg DelegateEventWithAnArg;
        event VoidDelegateWithMultipleArgs DelegateEventWithMultipleArgs;
        event FuncDelegateWithoutArgs FuncDelegate;
        event FuncDelegateWithArgs FuncDelegateWithArgs;
        event EventHandler StandardEventHandler;
        event EventHandler<EventArgs> StandardGenericEventHandler;
        event EventHandler<CustomEventArgs> EventHandlerWithCustomArgs;
        event EventHandler<CustomEventArgsWithNoDefaultCtor> EventHandlerWithCustomArgsAndNoDefaultCtor;
        event CustomEventThatDoesNotInheritFromEventHandler CustomEventThatDoesNotInheritFromEventHandler;
    }
    public delegate void VoidDelegateWithoutArgs();
    public delegate void VoidDelegateWithEventArgs(object sender, EventArgs args);
    public delegate void VoidDelegateWithAnArg(int arg);
    public delegate void VoidDelegateWithMultipleArgs(int intArg, string stringArg);
    public delegate int FuncDelegateWithoutArgs();
    public delegate int FuncDelegateWithArgs(int intArg, string stringArg);
    public delegate void CustomEventThatDoesNotInheritFromEventHandler(object sender, CustomEventArgs args);
    public class CustomEventArgs : EventArgs { }
#pragma warning disable CS9113 // Parameter is unread.
    public class CustomEventArgsWithNoDefaultCtor(string arg) : EventArgs
#pragma warning restore CS9113 // Parameter is unread.
    {
    }

    public class CustomEventArgsWithInternalDefaultConstructor : EventArgs
    {
        internal CustomEventArgsWithInternalDefaultConstructor() { }
    }
    public interface IExampleInternal
    {
        public event EventHandler<CustomEventArgsWithInternalDefaultConstructor> MyEvent;
    }
    public class ConsumerInternal
    {
        public ConsumerInternal(IExampleInternal example)
        {
            example.MyEvent += OnMyEvent;
        }
        public bool SomethingWasDone { get; private set; }
        private void OnMyEvent(object sender, CustomEventArgsWithInternalDefaultConstructor args)
        {
            DoSomething();
        }
        private void DoSomething()
        {
            SomethingWasDone = true;
        }
    }

    public class CustomEventArgsWithPrivateDefaultConstructor : EventArgs
    {
        private CustomEventArgsWithPrivateDefaultConstructor() { }
    }
    public interface IExamplePrivate
    {
        public event EventHandler<CustomEventArgsWithPrivateDefaultConstructor> MyEvent;
    }
}