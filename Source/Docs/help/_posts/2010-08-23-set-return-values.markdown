---
title: Setting return values
layout: post
---

The following examples relate to substituting for the following interface:

{% examplecode csharp %}
public interface ICalculator
{
	int Add(int a, int b);
	string Mode { get; set; }
}
{% endexamplecode %}

{% requiredcode %}
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
{% endrequiredcode %}

## For methods
To set a return value for a method call on a substitute, call the method as normal, then follow it with a call to NSubstitute's `Returns()` extension method.

{% examplecode csharp %}
var calculator = Substitute.For<ICalculator>();
calculator.Add(1, 2).Returns(3);
{% endexamplecode %}

This value will be returned every time this call is made. `Returns()` will only apply to this combination of arguments, so other calls to this method will return a default value instead.

{% examplecode csharp %}
//Make a call return 3:
calculator.Add(1, 2).Returns(3); 
Assert.AreEqual(calculator.Add(1, 2), 3);
Assert.AreEqual(calculator.Add(1, 2), 3);

//Call with different arguments does not return 3
Assert.AreNotEqual(calculator.Add(3, 6), 3); 
{% endexamplecode %}

## For properties
The return value for a property can be set in the same was as for a method, using `Returns()`. You can also just use plain old property setters for read/write properties; they'll behave just the way you expect them to.

{% examplecode csharp %}
calculator.Mode.Returns("DEC");
Assert.AreEqual(calculator.Mode, "DEC");

calculator.Mode = "HEX";
Assert.AreEqual(calculator.Mode, "HEX");
{% endexamplecode %}

## Multiple return values
A call can also be configured to return a different value over multiple calls. The following example shows this for a call to a property, but it works the same way for method calls.

{% examplecode csharp %}
calculator.Mode.Returns("DEC", "HEX", "BIN");
Assert.AreEqual("DEC", calculator.Mode);
Assert.AreEqual("HEX", calculator.Mode);
Assert.AreEqual("BIN", calculator.Mode);
{% endexamplecode %}

## Return value for argument combinations
Return values can be configured for different combinations of arguments passed to calls using [argument matchers](/help/argument-matchers). This topic is covered in more detail in the [Argument matchers](/help/argument-matchers) entry, but the following examples show the general idea.

{% examplecode csharp %}
//Any first arg, 5 as second arg:
calculator.Add(Arg.Any<int>(), 5).Returns(10);
Assert.AreEqual(10, calculator.Add(123, 5));
Assert.AreEqual(10, calculator.Add(-9, 5));
Assert.AreNotEqual(10, calculator.Add(-9, -9));

//1 as first arg, second arg less than 0:
calculator.Add(1, Arg.Is<int>(x => x < 0)).Returns(345);
Assert.AreEqual(345, calculator.Add(1, -2));
Assert.AreNotEqual(345, calculator.Add(1, 2));

//Both args equal to 0:
calculator.Add(Arg.Is(0), Arg.Is(0)).Returns(99);
Assert.AreEqual(99, calculator.Add(0, 0));
{% endexamplecode %}

## Return value for any arguments
A call can be configured to return a value regardless of the arguments passed using the `ReturnsForAnyArgs()` extension method.

{% examplecode csharp %}
calculator.Add(1, 2).ReturnsForAnyArgs(100); 
Assert.AreEqual(calculator.Add(1, 2), 100);
Assert.AreEqual(calculator.Add(-7, 15), 100);
{% endexamplecode %}

The same behaviour can also be achieved using [argument matchers](/help/argument-matchers): it is simply a shortcut for replacing each argument with `Arg.Any<T>()`.

`ReturnsForAnyArgs()` has the same overloads as `Returns()`, so you can also specify multiple return values or calculated return values using this approach.

## Calculating the return value with a function
The return value for a call to a property or method can be set to the result of a function. This allows more complex logic to be put into the substitute. Although this is normally a bad practice, there are some situations in which it is useful.

{% examplecode csharp %}
calculator
    .Add(Arg.Any<int>(), Arg.Any<int>())
    .Returns(x => (int)x[0] + (int)x[1]);

Assert.That(calculator.Add(1, 1), Is.EqualTo(2));
Assert.That(calculator.Add(20, 30), Is.EqualTo(50));
Assert.That(calculator.Add(-73, 9348), Is.EqualTo(9275));
{% endexamplecode %}

In this example [argument matchers](/help/argument-matchers) are used to match all calls to `Add()`, and a lambda function is used to return the sum of the first and second arguments passed to the call.

### Call information
The function we provide to `Returns()` and `ReturnsForAnyArgs()` is of type `Func<CallInfo,T>`, where `T` is the type the call is returning, and `CallInfo` is a type which provides access to the arguments used for the call. In the previous example we accessed these arguments using an indexer (`x[1]` for the second argument). `CallInfo` also has a convenience method to pick arguments in a strongly typed way: 

{% examplecode csharp %}
public interface IFoo {
    string Bar(int a, string b);
}
{% endexamplecode %}

{% examplecode csharp %}
var foo = Substitute.For<IFoo>();
foo.Bar(0, "").ReturnsForAnyArgs(x => "Hello " + x.Arg<string>());
Assert.That(foo.Bar(1, "World"), Is.EqualTo("Hello World"));
{% endexamplecode %}

Here `x.Arg<string>()` will return the `string` argument passed to the call, rather than having to use `(string) x[1]`. If there are two `string` arguments to a call, NSubstitute will throw an exception and let you know that it can't work out which argument you mean.

### Callbacks

This technique can also be used to get a callback whenever a call is made:

{% examplecode csharp %}
var counter = 0;
calculator
    .Add(0, 0)
    .ReturnsForAnyArgs(x => {
        counter++;
        return 0;
    });

calculator.Add(7,3);
calculator.Add(2,2);
calculator.Add(11,-3);
Assert.AreEqual(counter, 3);
{% endexamplecode %}
