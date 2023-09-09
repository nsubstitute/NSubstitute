---
title: Actions with argument matchers
layout: post
---

In addition to [specifying calls](/help/argument-matchers), matchers can also be used to perform a specific action with an argument whenever a matching call is made to a substitute. This is a fairly rare requirement, but can make test setup a little simpler in some cases.

_Warning:_ Once we start adding non-trivial behaviour to our substitutes we run the risk of over-specifying or tightly coupling our tests and code. It may be better to pick a different abstraction that better encapsulates this behaviour, or even use a real collaborator and switch to coarser grained tests for the class being tested.

## Invoking callbacks

Say our class under test needs to call a method on a dependency, and provide a callback so it can be notified when the dependent object has finished. We can use `Arg.Invoke()` to immediately invoke the callback argument as soon as the substitute is called.

Let's look at a contrived example. Say we are testing an `OrderPlacedCommand`, which needs to use an `IOrderProcessor` to process the order, then raise and event using `IEvents` when this completes successfully.

<!--
```requiredcode
public interface ICart { int OrderId { get; set; } }
public interface IEvents { void RaiseOrderProcessed(int orderId); }
```
-->

```csharp
public interface IOrderProcessor {
    void ProcessOrder(int orderId, Action<bool> orderProcessed);
}

public class OrderPlacedCommand {
    IOrderProcessor orderProcessor;
    IEvents events;
    public OrderPlacedCommand(IOrderProcessor orderProcessor, IEvents events) {
        this.orderProcessor = orderProcessor;
        this.events = events;
    }
    public void Execute(ICart cart) {
        orderProcessor.ProcessOrder(
            cart.OrderId, 
            wasOk => { if (wasOk) events.RaiseOrderProcessed(cart.OrderId); }
        );
    }
}
```

In our test we can use `Arg.Invoke` to simulate the situation where the `IOrderProcessor` finishes processing the order and invokes the callback to tell the calling code it is finished.

```csharp
[Test]
public void Placing_order_should_raise_order_processed_when_processing_is_successful() {
    //Arrange
    var cart = Substitute.For<ICart>();
    var events = Substitute.For<IEvents>();
    var processor = Substitute.For<IOrderProcessor>();
    cart.OrderId = 3;
    //Arrange for processor to invoke the callback arg with `true` whenever processing order id 3
    processor.ProcessOrder(3, Arg.Invoke(true));

    //Act
    var command = new OrderPlacedCommand(processor, events);
    command.Execute(cart);

    //Assert
    events.Received().RaiseOrderProcessed(3);
}
```

Here we setup the `processor` to invoke the callback whenever processing an order with id 3. We set it up to pass `true` to this callback using `Arg.Invoke(true)`.

There are several overloads to `Arg.Invoke` that let us invoke callbacks with varying numbers and types of arguments. We can also invoke custom delegate types (those that are not just simple `Action` delegates) using `Arg.InvokeDelegate`.

## Performing actions with arguments

Sometimes we may not want to invoke a callback immediately. Or maybe we want to store all instances of a particular argument passed to a method. Or even just capture a single argument for inspection later. We can use `Arg.Do` for these purposes. `Arg.Do` calls the action we give it with the argument used for each matching call.

<!--
```requiredcode
public interface ICalculator { int Multiply(int a, int b); }
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

```csharp
var argumentUsed = 0;
calculator.Multiply(Arg.Any<int>(), Arg.Do<int>(x => argumentUsed = x));

calculator.Multiply(123, 42);

Assert.AreEqual(42, argumentUsed);
```

Here we specify that a call to `Multiply` with any first argument should pass the second argument and put it in the `argumentUsed` variable. This can be quite useful when we want to assert several properties on an argument (for types more complex than `int` that is).

```csharp
var firstArgsBeingMultiplied = new List<int>();
calculator.Multiply(Arg.Do<int>(x => firstArgsBeingMultiplied.Add(x)), 10);

calculator.Multiply(2, 10);
calculator.Multiply(5, 10);
calculator.Multiply(7, 4567); //Will not match our Arg.Do as second arg is not 10

Assert.AreEqual(firstArgsBeingMultiplied, new[] { 2, 5 });
```

Here our `Arg.Do` takes whatever `int` is passed as the first argument to `Multiply` and adds it to a list whenever the second argument is 10.

## Argument actions and call specification

Argument actions act just like the [`Arg.Any<T>()` argument matcher](/help/argument-matchers) in that they specify a call where that argument is any type compatible with `T` (and so can be used for [setting return values](/help/return-for-args) and [checking received calls](/help/received-calls)). They just have the added element of interacting with a specific argument of any call that matches that specification.

```csharp
var numberOfCallsWhereFirstArgIsLessThan0 = 0;
//Specify a call where the first arg is less than 0, and the second is any int.
//When this specification is met we'll increment a counter in the Arg.Do action for 
//the second argument that was used for the call, and we'll also make it return 123.
calculator
    .Multiply(
        Arg.Is<int>(x => x < 0), 
        Arg.Do<int>(x => numberOfCallsWhereFirstArgIsLessThan0++)
    ).Returns(123);

var results = new[] {
    calculator.Multiply(-4, 3),
    calculator.Multiply(-27, 88),
    calculator.Multiply(-7, 8),
    calculator.Multiply(123, 2) //First arg greater than 0, so spec won't be met.
};

Assert.AreEqual(3, numberOfCallsWhereFirstArgIsLessThan0); //3 of 4 calls have first arg < 0
Assert.AreEqual(results, new[] {123, 123, 123, 0}); //Last call returns 0, not 123
```