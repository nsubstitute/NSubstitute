---
title: Checking received calls
layout: post
---

In some cases (particularly for `void` methods) it is useful to check that a specific call has been received by a substitute. This can be checked using the `Received()` extension method, followed by the call being checked.

{% examplecode csharp %}
public interface ICommand {
    void Execute();
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
{% endexamplecode %}

In this case `command` did receive a call to `Execute()`, and so will complete successfully. If `Execute()` has not been received NSubstitute will throw a `CallNotReceivedException` and let you know what call was expected and with which arguments, as well as listing actual calls to that method and which the arguments differed.

## Check a call was not received
NSubstitute can also make sure a call was not received using the `DidNotReceive()` extension method.

{% examplecode csharp %}
var command = Substitute.For<ICommand>();
var something = new SomethingThatNeedsACommand(command);
//Act
something.DontDoAnything();
//Assert
command.DidNotReceive().Execute();
{% endexamplecode %}

## Received (or not) with specific arguments

{% requiredcode %}
public interface ICalculator {
    int Add(int a, int b);
    int Subtract(int a, int b);
}
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
{% endrequiredcode %}

We can also use [argument matchers](/help/argument-matchers) to check calls were received (or not) with particular arguments. This is covered in more detail in the [argument matchers](/help/argument-matchers) topic, but the following examples show the general idea:

{% examplecode csharp %}
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
{% endexamplecode %}

## Ignoring arguments

NSubstitute can also check calls were received or not received but ignore the arguments used, just like we can for [setting returns for any arguments](/help/return-for-any-args). In this case we need `ReceivedWithAnyArgs()` and `DidNotReceiveWithAnyArgs()`.

{% examplecode csharp %}
calculator.Add(1, 3);

calculator.ReceivedWithAnyArgs().Add(1,1);
calculator.DidNotReceiveWithAnyArgs().Subtract(0,0);
{% endexamplecode %}



