---
title: Getting started
layout: post
---

The easiest way to get started is to reference NSubstitute from your test project using the [NSubstitute NuGet package](http://nuget.org/List/Packages/NSubstitute) via [NuGet](http://nuget.codeplex.com/wikipage?title=Getting%20Started) or [OpenWrap](https://github.com/openrasta/openwrap/wiki/Nuget). Alternatively you can [download NSubstitute](http://github.com/nsubstitute/NSubstitute/downloads) and add a reference to the `NSubstitute.dll` file included in the download into your test project.

So now you are staring at a blank test fixture (created with your favourite unit testing framework; for these examples we're using [NUnit](http://www.nunit.org/)), and are wondering where to start. 

First, add `using NSubstitute;` to your current C# file. This will give you everything you need to start substituting. 

Now let's say we have a basic calculator interface:

{% examplecode csharp %}
public interface ICalculator
{
    int Add(int a, int b);
    string Mode { get; set; }
    event EventHandler PoweringUp;
}
{% endexamplecode %}

{% requiredcode %}
ICalculator calculator;
[SetUp]
public void SetUp() { calculator = Substitute.For<ICalculator>(); }
{% endrequiredcode %}

We can ask NSubstitute to create a substitute instance for this type. We could ask for a stub, mock, fake, spy, test double etc., but why bother when we just want to substitute an instance we have some control over?

{% examplecode csharp %}
calculator = Substitute.For<ICalculator>();
{% endexamplecode %}

Now we can tell our substitute to return a value for a call:

{% examplecode csharp %}
calculator.Add(1, 2).Returns(3);
Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
{% endexamplecode %}

We can check that our substitute received a call, and did not receive others:

{% examplecode csharp %}
calculator.Add(1, 2);
calculator.Received().Add(1, 2);
calculator.DidNotReceive().Add(5, 7);
{% endexamplecode %}

If our `Received()` assertion fails, NSubstitute tries to give us some help as to what the problem might be:


    NSubstitute.Exceptions.ReceivedCallsException : Expected to receive a call matching:
        Add(1, 2)
    Actually received no matching calls.
    Received 2 non-matching calls (non-matching arguments indicated with '*' characters):
        Add(*4*, *7*)
        Add(1, *5*)

We can also work with properties using the `Returns` syntax we use for methods, or just stick with plain old property setters (for read/write properties):

{% examplecode csharp %}
calculator.Mode.Returns("DEC");
Assert.That(calculator.Mode, Is.EqualTo("DEC"));

calculator.Mode = "HEX";
Assert.That(calculator.Mode, Is.EqualTo("HEX"));
{% endexamplecode %}

NSubstitute supports [argument matching](/help/argument-matchers/) for setting return values and asserting a call was received:

{% examplecode csharp %}
calculator.Add(10, -5);
calculator.Received().Add(10, Arg.Any<int>());
calculator.Received().Add(10, Arg.Is<int>(x => x < 0));
{% endexamplecode %}

We can use argument matching as well as passing a function to `Returns()` to get some more behaviour out of our substitute (possibly too much, but that's your call):

{% examplecode csharp %}
calculator
   .Add(Arg.Any<int>(), Arg.Any<int>())
   .Returns(x => (int)x[0] + (int)x[1]);
Assert.That(calculator.Add(5, 10), Is.EqualTo(15));
{% endexamplecode %}

`Returns()` can also be called with multiple arguments to set up a sequence of return values.

{% examplecode csharp %}
calculator.Mode.Returns("HEX", "DEC", "BIN");
Assert.That(calculator.Mode, Is.EqualTo("HEX"));
Assert.That(calculator.Mode, Is.EqualTo("DEC"));
Assert.That(calculator.Mode, Is.EqualTo("BIN"));
{% endexamplecode %}

Finally, we can raise events on our substitutes (unfortunately C# dramatically restricts the extent to which this syntax can be cleaned up):

{% examplecode csharp %}
bool eventWasRaised = false;
calculator.PoweringUp += (sender, args) => eventWasRaised = true;

calculator.PoweringUp += Raise.Event();
Assert.That(eventWasRaised);
{% endexamplecode %}

That's pretty much all you need to get started with NSubstitute. Read on for more detailed feature descriptions, as well as for some of the less common requirements that NSubstitute supports.
