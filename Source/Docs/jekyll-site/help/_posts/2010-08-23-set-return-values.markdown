---
title: Setting return values
layout: post
---

The following examples related to substituting for the following interface:

{% examplecode csharp %}
public interface ICalculator
{
	int Add(int a, int b);
	string Mode { get; set; }
}
{% endexamplecode %}

## For methods

To set a return value for a method call on a substitute, call the method as normal, then follow it with a call to NSubstitute's `Returns` extension method.

{% examplecode csharp %}
var calculator = Substitute.For<ICalculator>();
calculator.Add(1, 2).Returns(3); //Make this call return 3.
{% endexamplecode %}

This will only apply to this combination of arguments, so other calls to this method will return a default value instead.

{% examplecode csharp %}
var calculator = Substitute.For<ICalculator>();
calculator.Add(1, 2).Returns(3); //Make this call return 3.
Assert.AreEqual(calculator.Add(1, 2), 3);
Assert.AreEqual(calculator.Add(1, 2), 3);
//Call with different arguments does not return 3
Assert.AreNotEqual(calculator.Add(3, 6), 3); 
{% endexamplecode %}

