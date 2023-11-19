---
title: Compatibility argument matchers
layout: post
---

NSubstitute [argument matchers](/help/argument-matchers) depend on having C# 7.0 or later (as of NSubstitute 4.0). This lets them be used with `out` and `ref` parameters, but it also means that if you are stuck on an earlier version of C# you may get an error like the following when trying to use a matcher like `Arg.Is(123)`:

> `CS7085: By-reference return type 'ref T' is not supported.`

If you have C# 7.0-compatible tooling installed you can [set `<LangVersion />` in your test csproj file](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version) to `7` or higher, or to `latest` or `default`.

Stuck with pre-7.0 tooling? Then use `Arg.Compat` instead of `Arg`, or use `CompatArg` in the `NSubstitute.Compatibility` namespace. `Arg.Compat` will work everywhere `Arg` does, with the exception of matching `out` and `ref` args. For example, if the documentation mentions `Arg.Is(42)`, you can instead use `Arg.Compat.Is(42)`. `CompatArg` is a bit trickier to setup, but may make migrating between `Arg` and `Arg.Compat` easier in some cases. Both options are described below.

<!--
```requiredcode
public interface ICalculator {
    int Add(int a, int b);
}
ICalculator calculator;
[SetUp] public void SetUp() { 
    calculator = Substitute.For<ICalculator>(); 
}
```
-->

## Using `Arg.Compat`

The simplest work-around if you are stuck on pre-C#7 is to use `Arg.Compat.` wherever you would normally use `Arg.`. To migrate existing code, replace all instances of `Arg.` with `Arg.Compat.`.

```csharp
calculator.Add(1, -10);

// Instead of `Arg.Is<int>(x => x < 0)`, use:
calculator.Received().Add(1, Arg.Compat.Is<int>(x => x < 0));

// Instead of `Arg.Any<int>()`, use:
calculator
    .Received()
    .Add(1, Arg.Compat.Any<int>());

// Same for Returns and DidNotReceive:
calculator.Add(Arg.Compat.Any<int>(), Arg.Compat.Is(42)).Returns(123);
calculator.DidNotReceive().Add(Arg.Compat.Is<int>(x => x > 10), -10);
```

## Using `NSubstitute.Compatibility.CompatArg`

If you have a project with lots of existing arg matchers then migrating to `Arg.Compat` can require a lot of code changes. A smaller change is to instead use an instance of the `CompatArg` class in the `NSubstitute.Compatibility` namespace. This approach may also make it easier to upgrade to the newer `Arg` matchers in future.

```csharp
[TestFixture]
public class SampleCompatArgFixture {

    // Declare Arg field. Any existing `Arg` references will now go via `CompatArg`, instead
    // of the new `Arg` type that is incompatible with older C# compilers.
    private static readonly NSubstitute.Compatibility.CompatArg Arg = NSubstitute.Compatibility.CompatArg.Instance;

    [Test]
    public void DemonstrationOfCompatArgs() {
        var calculator = Substitute.For<ICalculator>();

        calculator.Add(1, -10);

        // Arg.Is will now go via CompatArg. It is equivalent to Arg.Compat.Is.
        calculator.Received().Add(1, Arg.Is<int>(x => x < 0));
    }
}
```

This works particularly well if a common test base class is used.

```csharp
public class BaseTestFixture {

    // Declare Arg field. Any existing `Arg` references will now go via `CompatArg`, instead
    // of the new `Arg` type that is incompatible with older C# compilers.
    protected static readonly NSubstitute.Compatibility.CompatArg Arg = NSubstitute.Compatibility.CompatArg.Instance;

}
```

If you are later able to update the C# compiler your project is using, you can remove the `CompatArg` field and all `Arg` references will go through standard arg matchers (and you'll now be able to use them with `out` and `ref` parameters!).

## Argument matchers in expression trees

As of NSubstitute 4.0 argument matchers can no longer be used in expression trees. Doing so will cause a compilation error:

> CS8153: An expression tree lambda may not contain a call to a method, property, or indexer that returns by reference

The `Arg.Compat` matchers can be used to work around this issue. They do not return by reference so they are fine to use in expression trees. 

<!--
```requiredcode
// Pretending this takes an Expression<Action> arg (rather than adding a reference to
// System.Linq.Expression).
protected void specify(Action expectation) {
    expectation();
}
```
-->

```csharp
public interface IFoo { void DoStuff(int i); }

[Test]
public void Sample() {
    var sub = Substitute.For<IFoo>();
    sub.DoStuff(42);
    // If `specify` takes an `Expression<Action>` argument, this will fail with CS8153: 
    //   specify(() => sub.Received().DoStuff(Arg.Any<int>()));

    // Instead use compat matcher:
    specify(() => sub.Received().DoStuff(Arg.Compat.Any<int>()));

    // Or re-write without expression tree if possible:
    sub.Received().DoStuff(Arg.Any<int>());
}
```