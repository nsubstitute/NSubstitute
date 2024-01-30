---
title: Checking received calls
layout: post
---

In some cases (particularly for `void` methods) it is useful to check that a specific call has been received by a substitute. This can be checked using the `Received()` extension method, followed by the call being checked.

```csharp
public interface ICommand {
    void Execute();
    event EventHandler Executed;
}

public class SomethingThatNeedsACommand {
    ICommand command;
    public SomethingThatNeedsACommand(ICommand command) { 
        this.command = command;
    }
    public void DoSomething() { command.Execute(); }
    public void DontDoAnything() { }
}

[Test]
public void Should_execute_command() {
    //Arrange
    var command = Substitute.For<ICommand>();
    var something = new SomethingThatNeedsACommand(command);
    //Act
    something.DoSomething();
    //Assert
    command.Received().Execute();
}
```

In this case `command` did receive a call to `Execute()`, and so will complete successfully. If `Execute()` has not been received NSubstitute will throw a `ReceivedCallsException` and let you know what call was expected and with which arguments, as well as listing actual calls to that method and which the arguments differed.

## Check a call was not received
NSubstitute can also make sure a call was not received using the `DidNotReceive()` extension method.

```csharp
var command = Substitute.For<ICommand>();
var something = new SomethingThatNeedsACommand(command);
//Act
something.DontDoAnything();
//Assert
command.DidNotReceive().Execute();
```

## Check a call was received a specific number of times

The `Received()` extension method will assert that at least one call was made to a member, and `DidNotReceive()` asserts that zero calls were made. NSubstitute also gives you the option of asserting a specific number of calls were received by passing an integer to `Received()`. This will throw if the substitute does not receive *exactly* that many matching calls. Too few, or too many, and the assertion will fail.

```csharp
public class CommandRepeater {
    ICommand command;
    int numberOfTimesToCall;
    public CommandRepeater(ICommand command, int numberOfTimesToCall) {
      this.command = command;
      this.numberOfTimesToCall = numberOfTimesToCall;
    }

    public void Execute() { 
      for (var i=0; i<numberOfTimesToCall; i++) command.Execute();
    }
}

[Test]
public void Should_execute_command_the_number_of_times_specified() {
  var command = Substitute.For<ICommand>();
  var repeater = new CommandRepeater(command, 3);
  //Act
  repeater.Execute();
  //Assert
  command.Received(3).Execute(); // << This will fail if 2 or 4 calls were received
}
```

We can also use `Received(1)` to check a call was received once and only once. This differs from the standard `Received()` call, which checks a call was received *at least* once. `Received(0)` behaves the same as `DidNotReceive()`.

## Received (or not) with specific arguments

<!--
```requiredcode
public interface ICalculator {
    int Add(int a, int b);
    int Subtract(int a, int b);
    string Mode { get; set; }
}
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

We can also use [argument matchers](/help/argument-matchers) to check calls were received (or not) with particular arguments. This is covered in more detail in the [argument matchers](/help/argument-matchers) topic, but the following examples show the general idea:

```csharp
calculator.Add(1, 2);
calculator.Add(-100, 100);

//Check received with second arg of 2 and any first arg:
calculator.Received().Add(Arg.Any<int>(), 2);
//Check received with first arg less than 0, and second arg of 100:
calculator.Received().Add(Arg.Is<int>(x => x < 0), 100);
//Check did not receive a call where second arg is >= 500 and any first arg:
calculator
    .DidNotReceive()
    .Add(Arg.Any<int>(), Arg.Is<int>(x => x >= 500));
```

## Ignoring arguments

NSubstitute can also check calls were received or not received but ignore the arguments used, just like we can for [setting returns for any arguments](/help/return-for-any-args). In this case we need `ReceivedWithAnyArgs()` and `DidNotReceiveWithAnyArgs()`.

```csharp
calculator.Add(1, 3);

calculator.ReceivedWithAnyArgs().Add(default, default);
calculator.DidNotReceiveWithAnyArgs().Subtract(default, default);
```

## Checking calls to properties

The same syntax can be used to check calls on properties. Normally we'd want to avoid this, as we're really more interested in testing the required behaviour rather than the precise implementation details (i.e. we would set the property to return a value and check that was used properly, rather than assert that the property getter was called). Still, there are probably times when checking getters and setters were called can come in handy, so here's how you do it:

```csharp
var mode = calculator.Mode;
calculator.Mode = "TEST";

//Check received call to property getter
//We need to assign the result to a variable to keep
//the compiler happy or use discards (since C# 7.0).
_ = calculator.Received().Mode;

//Check received call to property setter with arg of "TEST"
calculator.Received().Mode = "TEST";
```

## Checking calls to indexers
An indexer is really just another property, so we can use the same syntax to check calls to indexers.

```csharp 
var dictionary = Substitute.For<IDictionary<string, int>>();
dictionary["test"] = 1;

dictionary.Received()["test"] = 1;
dictionary.Received()["test"] = Arg.Is<int>(x => x < 5);
```

## Checking event subscriptions

As with properties, we'd normally favour testing the required behaviour over checking subscriptions to particular event handlers. We can do that by [raising an event on the substitute](/help/raising-events/) and asserting our class performs the correct behaviour in response:

```csharp 
public class CommandWatcher {
    ICommand command;
    public CommandWatcher(ICommand command) { 
        command.Executed += OnExecuted;
    }
    public bool DidStuff { get; private set; }
    public void OnExecuted(object o, EventArgs e) { DidStuff = true; }
} 

[Test]
public void ShouldDoStuffWhenCommandExecutes() {
  var command = Substitute.For<ICommand>();
  var watcher = new CommandWatcher(command);

  command.Executed += Raise.Event();

  Assert.That(watcher.DidStuff);
}
``` 

If required though, `Received` will let us assert that the subscription was received:

```csharp
[Test]
public void MakeSureWatcherSubscribesToCommandExecuted() {
    var command = Substitute.For<ICommand>();
    var watcher = new CommandWatcher(command);

    // Not recommended. Favour testing behaviour over implementation specifics.
    // Can check subscription:
    command.Received().Executed += watcher.OnExecuted;
    // Or, if the handler is not accessible:
    command.Received().Executed += Arg.Any<EventHandler>();
}
```

## Checking event invocation

We can also use substitutes for event handlers to confirm that a particular event was raised correctly. Often a simple lambda function will suffice, but if we want to use argument matchers we can use a substitute and `Received`. Both options are shown below:

```csharp 
public class LowFuelWarningEventArgs : EventArgs {
    public int PercentLeft { get; }
    public LowFuelWarningEventArgs(int percentLeft){
        PercentLeft = percentLeft;
    }
}

public class FuelManagement{
    public event EventHandler<LowFuelWarningEventArgs> LowFuelDetected;
    public void DoSomething(){
        LowFuelDetected?.Invoke(this, new LowFuelWarningEventArgs(15));
    }
}

// Often it is easiest to use a lambda for this, as shown in the following test:
[Test]
public void ShouldRaiseLowFuel_WithoutNSub(){
    var fuelManagement = new FuelManagement();
    var eventWasRaised = false;
    fuelManagement.LowFuelDetected += (o,e) => eventWasRaised = true;

    fuelManagement.DoSomething();

    Assert.That(eventWasRaised);
}

// We can also use NSubstitute for this if we want more involved argument matching logic.
// NSubstitute also gives us a descriptive message if the assertion fails which may be helpful in some cases.
// (For example, if the call was not received with the expected arguments, we'll get a list of the non-matching
// calls made to that member.)
//
// Note we could still use lambdas and standard assertions for this, but a substitute may be worth considering
// in some of these cases.
[Test]
public void ShouldRaiseLowFuel(){
    var fuelManagement = new FuelManagement();
    var handler = Substitute.For<EventHandler<LowFuelWarningEventArgs>>();
    fuelManagement.LowFuelDetected += handler;

    fuelManagement.DoSomething();

    handler
        .Received()
        .Invoke(fuelManagement, Arg.Is<LowFuelWarningEventArgs>(x => x.PercentLeft < 20));
}
```
