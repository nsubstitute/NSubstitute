# NSubstitute

A friendly substitute for .NET mocking libraries.

Perfect for those new to testing, and for others who would just like to to get their tests written with less noise and fewer lambdas.

## How to use

```csharp
// Create:
var calculator = Substitute.For<ICalculator>();

// Set a return value:
calculator.Add(1, 2).Returns(3);
Assert.AreEqual(3, calculator.Add(1, 2));

// Check received calls:
calculator.Received().Add(1, Arg.Any<int>());
calculator.DidNotReceive().Add(2, 2);

// Raise events
calculator.PoweringUp += Raise.Event();
```

## Optional Roslyn analysers

- For C# projects: [NSubstitute.Analyzers.CSharp](https://www.nuget.org/packages/NSubstitute.Analyzers.CSharp/)
- For VB projects: [NSubstitute.Analyzers.VisualBasic](https://www.nuget.org/packages/NSubstitute.Analyzers.VisualBasic/)

## Getting help

If you have questions, feature requests or feedback on NSubstitute please [raise an issue](https://github.com/nsubstitute/NSubstitute/issues) on our project site.

## More information

- Visit [NSubstitute website](https://nsubstitute.github.io) for more examples and documentation.
- Visit [NSubstitute on GitHub](https://github.com/nsubstitute/NSubstitute).