---
title: Multiple return values
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

A call can also be configured to return a different value over multiple calls. The following example shows this for a call to a property, but it works the same way for method calls.

```csharp
calculator.Mode.Returns("DEC", "HEX", "BIN");
Assert.That(calculator.Mode, Is.EqualTo("DEC"));
Assert.That(calculator.Mode, Is.EqualTo("HEX"));
Assert.That(calculator.Mode, Is.EqualTo("BIN"));
```

This can also be achieved by [returning from a function](/help/return-from-function), but passing multiple values to `Returns()` is simpler and reads better.

## Multiple returns using callbacks

`Returns()` also supports passing multiple [functions to return from](/help/return-from-function), which allows one call in a sequence to throw an exception or perform some other action.

```csharp
calculator.Mode.Returns(x => "DEC", x => "HEX", x => { throw new Exception(); });
Assert.That(calculator.Mode, Is.EqualTo("DEC"));
Assert.That(calculator.Mode, Is.EqualTo("HEX"));
Assert.Throws<Exception>(() => { var result = calculator.Mode; });
```

## Configuring other calls without using up multiple returns

If a call has been configured with multiple returns values, you can configure a more specific call without using up any of these callbacks using [`.Configure()`](/help/configure/).