---
title: Return from a function
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
The function we provide to `Returns()` and `ReturnsForAnyArgs()` is of type `Func<CallInfo,T>`, where `T` is the type the call is returning, and `CallInfo` is a type which provides access to the arguments used for the call. In the previous example we accessed these arguments using an indexer (`x[1]` for the second argument). `CallInfo` also has a couple of convenience methods to pick arguments in a strongly typed way:

* `T Arg<T>()`: Gets the argument of type `T` passed to this call.
* `T ArgAt<T>(int position)`: Gets the argument passed to this call at the specified zero-based position, converted to type `T`.

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

Alternatively the callback can be specified after the `Returns` using `AndDoes`:

{% examplecode csharp %}
var counter = 0;
calculator
    .Add(0, 0)
    .ReturnsForAnyArgs(x => 0)
    .AndDoes(x => counter++);

calculator.Add(7,3);
calculator.Add(2,2);
Assert.AreEqual(counter, 2);
{% endexamplecode %}
