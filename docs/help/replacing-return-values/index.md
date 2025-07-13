---
title: Replacing return values
---

The return value for a method or property can be set as many times as required. Only the most recently set value will be returned.

```csharp
calculator.Mode.Returns("DEC,HEX,OCT");
calculator.Mode.Returns(x => "???");
calculator.Mode.Returns("HEX");
calculator.Mode.Returns("BIN");
Assert.That(calculator.Mode, Is.EqualTo("BIN"));
```

<!--
```requiredcode
public interface ICalculator { string Mode { get; set; } }

ICalculator calculator;
[SetUp]
public void SetUp() {
    calculator = Substitute.For<ICalculator>();
}
```
-->