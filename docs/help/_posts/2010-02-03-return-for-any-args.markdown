---
title: Return for any args
layout: post
---

{% requiredcode %}
public interface ICalculator {
	int Add(int a, int b);
	string Mode { get; set; }
}
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
{% endrequiredcode %}

A call can be configured to return a value regardless of the arguments passed using the `ReturnsForAnyArgs()` extension method.

{% examplecode csharp %}
calculator.Add(1, 2).ReturnsForAnyArgs(100); 
Assert.AreEqual(100, calculator.Add(1, 2));
Assert.AreEqual(100, calculator.Add(-7, 15));
{% endexamplecode %}

The same behaviour can also be achieved using [argument matchers](/help/argument-matchers): it is simply a shortcut for replacing each argument with `Arg.Any<T>()`.

`ReturnsForAnyArgs()` has the same overloads as `Returns()`, so you can also specify multiple return values or calculated return values using this approach.
