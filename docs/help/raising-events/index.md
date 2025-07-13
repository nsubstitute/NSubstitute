---
title: Raising events
---

Sometimes it is necessary to raise events declared on the types being substituted for. Consider the following example:

```csharp

public interface IEngine {
    event EventHandler Idling;
    event EventHandler<LowFuelWarningEventArgs> LowFuelWarning;
    event Action<int> RevvedAt;
}

public class LowFuelWarningEventArgs : EventArgs {
    public int PercentLeft { get; private set; }
    public LowFuelWarningEventArgs(int percentLeft) {
        PercentLeft = percentLeft;
    }
}
```
<!--
```requiredcode
IEngine engine;
bool wasCalled;
int numberOfEvents;
[SetUp] public void SetUp() {
    engine = Substitute.For<IEngine>();
    wasCalled = false;
    numberOfEvents = 0;
    engine.Idling += (sender, args) => wasCalled = true;
}
```
-->

Events are "interesting" creatures in the .NET world, as you can't pass around references to them like you can with other members. Instead, you can only add or remove handlers to events, and it is this add handler syntax that NSubstitute uses to raise events.

```csharp
var wasCalled = false;
engine.Idling += (sender, args) => wasCalled = true;

//Tell the substitute to raise the event with a sender and EventArgs:
engine.Idling += Raise.EventWith(new object(), new EventArgs());

Assert.That(wasCalled);
```

In the example above we don't really mind what sender and `EventArgs` our event is raised with, just that it was called. In this case NSubstitute can make our life easier by creating the required arguments for our event handler:

```csharp
engine.Idling += Raise.Event();
Assert.That(wasCalled, Is.True);
```

## Raising events when arguments do not have a default constructor

NSubstitute will not always be able to create the event arguments for you. If your event args do not have a default constructor you will have to provide them yourself using `Raise.EventWith<TEventArgs>(...)`, as is the case for the `LowFuelWarning` event. NSubstitute will still create the sender for you if you don't provide it though.

```csharp
engine.LowFuelWarning += (sender, args) => numberOfEvents++;

//Raise event with specific args, any sender:
engine.LowFuelWarning += Raise.EventWith(new LowFuelWarningEventArgs(10));
//Raise event with specific args and sender:
engine.LowFuelWarning += Raise.EventWith(new object(), new LowFuelWarningEventArgs(10));

Assert.That(numberOfEvents, Is.EqualTo(2));
```

## Raising `Delegate` events

Sometimes events are declared with a _delegate_ that does not inherit from `EventHandler<T>` or `EventHandler`. These events can be raised using `Raise.Event<TypeOfEventHandlerDelegate>(arguments)`. NSubsitute will try and guess the arguments required for the delegate, but if it can't it will tell you what arguments you need to supply.

The following examples shows raising an `INotifyPropertyChanged` event, which uses a `PropertyChangedEventHandler` delegate and requires two parameters.

```csharp
var sub = Substitute.For<INotifyPropertyChanged>();
bool wasCalled = false;
sub.PropertyChanged += (sender, args) => wasCalled = true;

sub.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs("test"));

Assert.That(wasCalled);
```


## Raising `Action` events
In the `IEngine` example the `RevvedAt` event is declared as an `Action<int>`. This is another example of a delegate event, and we can use `Raise.Event<Action<int>>()` to raise our event.

```csharp
int revvedAt = 0;;
engine.RevvedAt += rpm => revvedAt = rpm;

engine.RevvedAt += Raise.Event<Action<int>>(123);

Assert.That(revvedAt, Is.EqualTo(123));
```