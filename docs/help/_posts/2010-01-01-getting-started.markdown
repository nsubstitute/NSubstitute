---
title: Getting started
layout: post
---

## Adding NSubstitute to your test project 

First add the [NSubstitute NuGet package](https://nuget.org/List/Packages/NSubstitute) to your test project using [NuGet](https://docs.microsoft.com/en-us/nuget/quickstart/use-a-package) (either the command line executable, or via the package manager in your IDE).

```sh
> Install-Package NSubstitute
```

It is optional but recommended to also install [NSubstitute.Analyzers.CSharp](https://www.nuget.org/packages/NSubstitute.Analyzers.CSharp/) for C# projects, or [NSubstitute.Analyzers.VisualBasic](https://www.nuget.org/packages/NSubstitute.Analyzers.VisualBasic/) for VB projects. NSubstitute will work without the analysers installed, but these packages will [help detect potential misuses of the NSubstitute API](/help/nsubstitute-analysers/).

```sh
> Install-Package NSubstitute.Analyzers.CSharp
// or
> Install-Package NSubstitute.Analyzers.VisualBasic
```
## Using NSubstitute in a test fixture

So now you are staring at a blank test fixture (created with your favourite unit testing framework; for these examples we're using [NUnit](https://nunit.org/)), and are wondering where to start. 

First, add `using NSubstitute;` to your current C# file. This will give you everything you need to start substituting. 

Now let's say we have a basic calculator interface:

```csharp
public interface ICalculator
{
    int Add(int a, int b);
    string Mode { get; set; }
    event EventHandler PoweringUp;
}
```

<!--
```requiredcode
ICalculator calculator;
[SetUp]
public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

We can ask NSubstitute to create a substitute instance for this type. We could ask for a stub, mock, fake, spy, test double etc., but why bother when we just want to substitute an instance we have some control over?

```csharp
calculator = Substitute.For<ICalculator>();
```

⚠️ **Note**: NSubstitute will only work properly with interfaces, or with class members that are overridable from the test assembly. Be very careful substituting for classes with non-virtual or `internal virtual` members, as real code could be inadvertently executed in your test. See [Creating a substitute](/help/creating-a-substitute/#substituting_infrequently_and_carefully_for_classes) and [How NSubstitute works](/help/how-nsub-works) for more information. Also make sure to install [NSubstitute.Analyzers](/help/nsubstitute-analysers) which can warn about many of these cases (but not all of them; be careful with classes!).

Now we can tell our substitute to return a value for a call:

```csharp
calculator.Add(1, 2).Returns(3);
Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
```

We can check that our substitute received a call, and did not receive others:

```csharp
calculator.Add(1, 2);
calculator.Received().Add(1, 2);
calculator.DidNotReceive().Add(5, 7);
```

If our `Received()` assertion fails, NSubstitute tries to give us some help as to what the problem might be:

```
NSubstitute.Exceptions.ReceivedCallsException : Expected to receive a call matching:
    Add(1, 2)
Actually received no matching calls.
Received 2 non-matching calls (non-matching arguments indicated with '*' characters):
    Add(*4*, *7*)
    Add(1, *5*)
```

We can also work with properties using the `Returns` syntax we use for methods, or just stick with plain old property setters (for read/write properties):

```csharp
calculator.Mode.Returns("DEC");
Assert.That(calculator.Mode, Is.EqualTo("DEC"));

calculator.Mode = "HEX";
Assert.That(calculator.Mode, Is.EqualTo("HEX"));
```

NSubstitute supports [argument matching](/help/argument-matchers/) for setting return values and asserting a call was received:

```csharp
calculator.Add(10, -5);
calculator.Received().Add(10, Arg.Any<int>());
calculator.Received().Add(10, Arg.Is<int>(x => x < 0));
```

We can use argument matching as well as passing a function to `Returns()` to get some more behaviour out of our substitute (possibly too much, but that's your call):

```csharp
calculator
   .Add(Arg.Any<int>(), Arg.Any<int>())
   .Returns(x => (int)x[0] + (int)x[1]);
Assert.That(calculator.Add(5, 10), Is.EqualTo(15));
```

`Returns()` can also be called with multiple arguments to set up a sequence of return values.

```csharp
calculator.Mode.Returns("HEX", "DEC", "BIN");
Assert.That(calculator.Mode, Is.EqualTo("HEX"));
Assert.That(calculator.Mode, Is.EqualTo("DEC"));
Assert.That(calculator.Mode, Is.EqualTo("BIN"));
```

Finally, we can raise events on our substitutes (unfortunately C# dramatically restricts the extent to which this syntax can be cleaned up):

```csharp
bool eventWasRaised = false;
calculator.PoweringUp += (sender, args) => eventWasRaised = true;

calculator.PoweringUp += Raise.Event();
Assert.That(eventWasRaised);
```

That's pretty much all you need to get started with NSubstitute. Read on for more detailed feature descriptions, as well as for some of the less common requirements that NSubstitute supports.
