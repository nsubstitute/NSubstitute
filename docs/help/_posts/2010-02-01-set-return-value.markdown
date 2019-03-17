---
title: Setting a return value
layout: post
---

The following examples relate to substituting for the following interface:

```csharp
public interface ICalculator {
	int Add(int a, int b);
	string Mode { get; set; }
}
```

<!--
```requiredcode
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

## For methods
To set a return value for a method call on a substitute, call the method as normal, then follow it with a call to NSubstitute's `Returns()` extension method.

```csharp
var calculator = Substitute.For<ICalculator>();
calculator.Add(1, 2).Returns(3);
```

This value will be returned every time this call is made. `Returns()` will only apply to this combination of arguments, so other calls to this method will return a default value instead.

```csharp
//Make a call return 3:
calculator.Add(1, 2).Returns(3);
Assert.AreEqual(calculator.Add(1, 2), 3);
Assert.AreEqual(calculator.Add(1, 2), 3);

//Call with different arguments does not return 3
Assert.AreNotEqual(calculator.Add(3, 6), 3);
```

## For properties
The return value for a property can be set in the same was as for a method, using `Returns()`. You can also just use plain old property setters for read/write properties; they'll behave just the way you expect them to.

```csharp
calculator.Mode.Returns("DEC");
Assert.AreEqual(calculator.Mode, "DEC");

calculator.Mode = "HEX";
Assert.AreEqual(calculator.Mode, "HEX");
```


## More ways of setting return values
This covers the very basics of setting a return value, but NSubstitute can do much more. Read on for other approachs, including [matching specific arguments](/help/return-for-args), [ignoring arguments](/help/return-for-any-args), using [functions to calculate return values](/help/return-from-function) and returning [multiple results](/help/multiple-returns).

