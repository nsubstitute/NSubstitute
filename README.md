NSubstitute
========
[![Build status](https://ci.appveyor.com/api/projects/status/ipe7ephhy6f9bbgp/branch/master?svg=true)](https://ci.appveyor.com/project/NSubstitute/nsubstitute/branch/master) [![Travis Build Status](https://travis-ci.com/nsubstitute/NSubstitute.svg?branch=master)](https://travis-ci.com/nsubstitute/NSubstitute)
[![Nuget](https://img.shields.io/nuget/v/NSubstitute.svg)](https://www.nuget.org/packages/NSubstitute)

Visit the [NSubstitute website](https://nsubstitute.github.io) for more information.

### What is it?

NSubstitute is designed as a friendly substitute for .NET mocking libraries.

It is an attempt to satisfy our craving for a mocking library with a succinct syntax that helps us keep the focus on the intention of our tests, rather than on the configuration of our test doubles. We've tried to make the most frequently required operations obvious and easy to use, keeping less usual scenarios discoverable and accessible, and all the while maintaining as much natural language as possible.

Perfect for those new to testing, and for others who would just like to to get their tests written with less noise and fewer lambdas.

### Installation

* [NSubstitute package](https://nuget.org/List/Packages/NSubstitute)
* Optional Roslyn analysers (recommended):
    * For C# projects: [NSubstitute.Analyzers.CSharp](https://www.nuget.org/packages/NSubstitute.Analyzers.CSharp/)
    * For VB projects: [NSubstitute.Analyzers.VisualBasic](https://www.nuget.org/packages/NSubstitute.Analyzers.VisualBasic/)

### Getting help

If you have questions, feature requests or feedback on NSubstitute please [raise an issue](https://github.com/nsubstitute/NSubstitute/issues) on our project site. All questions are welcome via our project site, but for "how-to"-style questions you can also try [StackOverflow with the \[nsubstitute\] tag](https://stackoverflow.com/tags/nsubstitute), which often leads to very good answers from the larger programming community. StackOverflow is especially useful if your question also relates to other libraries that our team may not be as familiar with (e.g. NSubstitute with Entity Framework). You can also head on over to the [NSubstitute discussion group](https://groups.google.com/group/nsubstitute) if you prefer.

### Basic use

Let's say we have a basic calculator interface:

```csharp
public interface ICalculator
{
    int Add(int a, int b);
    string Mode { get; set; }
    event Action PoweringUp;
}
```
<!--
```requiredcode
ICalculator _calculator;
[SetUp]
public void SetUp() { _calculator = Substitute.For<ICalculator>(); }
```
-->

We can ask NSubstitute to create a substitute instance for this type. We could ask for a stub, mock, fake, spy, test double etc., but why bother when we just want to substitute an instance we have some control over?

```csharp
_calculator = Substitute.For<ICalculator>();
```

⚠️ **Note**: NSubstitute will only work properly with interfaces or with `virtual` members of classes. Be careful substituting for classes with non-virtual members. See [Creating a substitute](https://nsubstitute.github.io/help/creating-a-substitute/#substituting_infrequently_and_carefully_for_classes) for more information.

Now we can tell our substitute to return a value for a call:

```csharp
_calculator.Add(1, 2).Returns(3);
Assert.That(_calculator.Add(1, 2), Is.EqualTo(3));
```

We can check that our substitute received a call, and did not receive others:

```csharp
_calculator.Add(1, 2);
_calculator.Received().Add(1, 2);
_calculator.DidNotReceive().Add(5, 7);
```

If our Received() assertion fails, NSubstitute tries to give us some help as to what the problem might be:


    NSubstitute.Exceptions.ReceivedCallsException : Expected to receive a call matching:
        Add(1, 2)
    Actually received no matching calls.
    Received 2 non-matching calls (non-matching arguments indicated with '*' characters):
        Add(1, *5*)
        Add(*4*, *7*)

We can also work with properties using the Returns syntax we use for methods, or just stick with plain old property setters (for read/write properties):

```csharp
_calculator.Mode.Returns("DEC");
Assert.That(_calculator.Mode, Is.EqualTo("DEC"));

_calculator.Mode = "HEX";
Assert.That(_calculator.Mode, Is.EqualTo("HEX"));
```

NSubstitute supports argument matching for setting return values and asserting a call was received:

```csharp
_calculator.Add(10, -5);
_calculator.Received().Add(10, Arg.Any<int>());
_calculator.Received().Add(10, Arg.Is<int>(x => x < 0));
```

We can use argument matching as well as passing a function to Returns() to get some more behaviour out of our substitute (possibly too much, but that's your call):

```csharp
_calculator
   .Add(Arg.Any<int>(), Arg.Any<int>())
   .Returns(x => (int)x[0] + (int)x[1]);
Assert.That(_calculator.Add(5, 10), Is.EqualTo(15));
```

Returns() can also be called with multiple arguments to set up a sequence of return values.

```csharp
_calculator.Mode.Returns("HEX", "DEC", "BIN");
Assert.That(_calculator.Mode, Is.EqualTo("HEX"));
Assert.That(_calculator.Mode, Is.EqualTo("DEC"));
Assert.That(_calculator.Mode, Is.EqualTo("BIN"));
```

Finally, we can raise events on our substitutes (unfortunately C# dramatically restricts the extent to which this syntax can be cleaned up):

```csharp
bool eventWasRaised = false;
_calculator.PoweringUp += () => eventWasRaised = true;

_calculator.PoweringUp += Raise.Event<Action>();
Assert.That(eventWasRaised);
```

### Building

NSubstitute and its tests can be compiled and run using Visual Studio and Visual Studio for Mac. Note that some tests are marked `[Pending]` and are not meant to pass at present, so it is a good idea to exclude tests in the Pending category from test runs.

There are also build scripts in the `./build` directory for command line builds, and CI configurations in the project root.

To do [full builds](https://github.com/nsubstitute/NSubstitute/wiki/Release-procedure) you'll also need Ruby, as the jekyll gem is used to generate the website.

### Other libraries you may be interested in

* [Moq](https://github.com/Moq/moq4/wiki/Quickstart): the original Arrange-Act-Assert mocking library for .NET, and a big source of inspiration for NSubstitute.
* [FakeItEasy](https://fakeiteasy.github.io/): another modern mocking library for .NET. If you're not sold on NSubstitute's syntax, try FIE!
* [substitute.js](https://github.com/ffMathy/FluffySpoon.JavaScript.Testing): a mocking library for TypeScript inspired by NSubstitute's syntax (`@fluffy-spoon/substitute` on NPM)

