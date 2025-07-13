---
title: Return for specific args
---

<!--
```requiredcode
public interface ICalculator {
    int Add(int a, int b);
    string Mode { get; set; }
}
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

Return values can be configured for different combinations of arguments passed to calls using [argument matchers](/help/argument-matchers). This topic is covered in more detail in the [Argument matchers](/help/argument-matchers) entry, but the following examples show the general idea.

```csharp
//Return when first arg is anything and second arg is 5:
calculator.Add(Arg.Any<int>(), 5).Returns(10);
Assert.That(calculator.Add(123, 5), Is.EqualTo(10));
Assert.That(calculator.Add(-9, 5), Is.EqualTo(10));
Assert.That(calculator.Add(-9, -9), Is.Not.EqualTo(10));

//Return when first arg is 1 and second arg less than 0:
calculator.Add(1, Arg.Is<int>(x => x < 0)).Returns(345);
Assert.That(345, Is.EqualTo(calculator.Add(1, -2)));
Assert.That(calculator.Add(1, 2), Is.Not.EqualTo(345));

//Return when both args equal to 0:
calculator.Add(Arg.Is(0), Arg.Is(0)).Returns(99);
Assert.That(99, Is.EqualTo(calculator.Add(0, 0)));
```