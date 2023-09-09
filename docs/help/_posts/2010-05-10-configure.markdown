---
title: Safe configuration and overlapping calls
layout: post
---

*`Configure()` is supported in NSubstitute 4.0 and above.*

Sometimes we want to configure a call that overlaps a more general call we have previously setup to run a [callback](/help/callbacks) or throw an [exception](/help/throwing-exceptions/). Ideally we would modify the setups so they don't overlap, but we can also prevent these callbacks from running while we setup the next call by calling `Configure()` before invoking the method we want to re-configure. `Configure()` tells NSubstitute we are configuring the following call so that it will not run any callbacks from previous configurations.

<!--
```requiredcode
public interface ICalculator { int Add(int a, int b); }
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

```csharp
calculator.Add(Arg.Any<int>(), Arg.Any<int>()).Returns(x => { throw new Exception(); });

// Now if we try to add a return value for a more specific returns this will throw
// before it gets a chance to configure the call:
//    calculator.Add(1, 2).Returns(3);

// Instead, we can use Configure to ensure the previous callback does not run:
calculator.Configure().Add(1, 2).Returns(3);

// Now both the exception callback and our other return have been configured:
Assert.AreEqual(3, calculator.Add(1, 2));
Assert.Throws<Exception>(() => calculator.Add(-2, -2));
```

NSubstitute will also assume we are configuring a call if we have an argument matcher in our call, such as `Arg.Is(1)` in `calculator.Add(Arg.Is(1), 2).Returns(3)`, but it is generally better to be more explicit by using `.Configure()`.

This can be particularly useful with [partial substitutes](/help/partial-subs/) to help avoid real code being executed when configuring calls that would otherwise call the base implementation.