---
title: Argument matchers
layout: post
---

Argument matchers can be used when [setting return values](/help/return-for-args) and when [checking received calls](/help/received-calls). They provide a way to _specify_ a call or group of calls, so that a return value can be set for all matching calls, or to check a matching call has been received.

{% requiredcode %}
public interface ICalculator {
    int Add(int a, int b);
    int Subtract(int a, int b);
}
public interface IFormatter {
  string Format(object o);
}
ICalculator calculator;
IFormatter formatter;
[SetUp] public void SetUp() { 
    calculator = Substitute.For<ICalculator>(); 
    formatter = Substitute.For<IFormatter>();
}
{% endrequiredcode %}

## Ignoring arguments
An argument of type `T` can be ignored using `Arg.Any<T>()`.

{% examplecode csharp %}
calculator.Add(Arg.Any<int>(), 5).Returns(7);

Assert.AreEqual(7, calculator.Add(42, 5));
Assert.AreEqual(7, calculator.Add(123, 5));
Assert.AreNotEqual(7, calculator.Add(1, 7));
{% endexamplecode %}

In this example we return `7` when adding any number to `5`. We use `Arg.Any<int>()` to tell NSubstitute to ignore the first argument.

We can also use this to match any argument of a specific sub-type.

{% examplecode csharp %}
formatter.Format(new object());
formatter.Format("some string");

formatter.Received().Format(Arg.Any<object>());
formatter.Received().Format(Arg.Any<string>());
formatter.DidNotReceive().Format(Arg.Any<int>());
{% endexamplecode %}

## Conditionally matching an argument
An argument of type `T` can be conditionally matched using `Arg.Is<T>(Predicate<T> condition)`.

{% examplecode csharp %}
calculator.Add(1, -10);

//Received call with first arg 1 and second arg less than 0:
calculator.Received().Add(1, Arg.Is<int>(x => x < 0));
//Received call with first arg 1 and second arg of -2, -5, or -10:
calculator
    .Received()
    .Add(1, Arg.Is<int>(x => new[] {-2,-5,-10}.Contains(x)));
//Did not receive call with first arg greater than 10:
calculator.DidNotReceive().Add(Arg.Is<int>(x => x > 10), -10);
{% endexamplecode %}

If the condition throws an exception for an argument, then it will be assumed that the argument does not match. The exception itself will be swallowed.

{% examplecode csharp %}
formatter.Format(Arg.Is<string>(x => x.Length <= 10)).Returns("matched");

Assert.AreEqual("matched", formatter.Format("short"));
Assert.AreNotEqual("matched", formatter.Format("not matched, too long"));
// Will not match because trying to access .Length on null will throw an exception when testing
// our condition. NSubstitute will assume it does not match and swallow the exception.
Assert.AreNotEqual("matched", formatter.Format(null));
{% endexamplecode %}

## Matching a specific argument
An argument of type `T` can be matched using `Arg.Is<T>(T value)`.

{% examplecode csharp %}
calculator.Add(0, 42);

//This won't work; NSubstitute isn't sure which arg the matcher applies to:
//calculator.Received().Add(0, Arg.Any<int>());

calculator.Received().Add(Arg.Is(0), Arg.Any<int>());
{% endexamplecode %}

This matcher normally isn't required; most of the time we can just use `0` instead of `Arg.Is(0)`. In some cases though, NSubstitute can't work out which matcher applies to which argument (arg matchers are actually fuzzily matched; not passed directly to the function call). In these cases it will throw an `AmbiguousArgumentsException` and ask you to specify one or more additional argument matchers. In some cases you may have to explicitly use argument matchers for every argument.

