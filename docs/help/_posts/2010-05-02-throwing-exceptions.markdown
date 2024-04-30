---
title: Throwing exceptions
layout: post
---

<!--
```requiredcode
public interface ICalculator { int Add(int a, int b); }
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

The `Throws` and `ThrowsAsync` helpers in the `NSubstitute.ExceptionExtensions` namespace can be used to throw exceptions when a member is called.

```csharp
//For non-voids:
calculator.Add(-1, -1).Throws(new Exception()); // Or .Throws<Exception>()

//For voids and non-voids:
calculator
    .When(x => x.Add(-2, -2))
    .Throw(x => new Exception()); // Or .Throw<Exception>() -  - don't use .Throw*s* in this case

//Both calls will now throw.
Assert.Throws<Exception>(() => calculator.Add(-1, -1));
Assert.Throws<Exception>(() => calculator.Add(-2, -2));
```

### Returns
Another way is to use the underlying method, `.Returns`. See also [Callbacks](/help/callbacks).

```csharp
//For non-voids:
calculator.Add(-1, -1).Returns(x => { throw new Exception(); });

//For voids and non-voids:
calculator
    .When(x => x.Add(-2, -2))
    .Do(x => { throw new Exception(); });

//Both calls will now throw.
Assert.Throws<Exception>(() => calculator.Add(-1, -1));
Assert.Throws<Exception>(() => calculator.Add(-2, -2));
```
