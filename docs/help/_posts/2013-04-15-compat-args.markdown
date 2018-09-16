---
title: Compatibility argument matchers
layout: post
---

NSubstitute [argument matchers](/help/argument-matchers) depend on having C# 7.0 or later (as of NSubstitute 4.0). If you are stuck on an earlier version of C# you may get an error like the following when trying to use a matcher like `Arg.Is(123)`:

> `CS7085: By-reference return type 'ref T' is not supported.`

If you have C# 7.0-compatible tooling installed you can [set `<LangVersion />` in your test csproj file](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version) to `7` or higher, or to `latest` or `default`.

Stuck with pre-7.0 tooling? Then use `CompatArg` from the `NSubstitute.Compatibility` namespace instead of `Arg`. `CompatArg` will work everywhere `Arg` does, with the exception of matching `out` and `ref` args (which require C# 7.0 features). For example, if the documentation mentions `Arg.Is(42)`, you can instead use `CompatArg.Is(42)`. 

## Using `CompatArg`

First, add `NSubstitute.Compatibility` to the list of imports:

    using NSubstitute;
    using NSubstitute.Compatibility;

Then use `CompatArg` in place of `Arg`:

{% requiredcode %}
public interface ICalculator {
    int Add(int a, int b);
}
ICalculator calculator;
[SetUp] public void SetUp() { 
    calculator = Substitute.For<ICalculator>(); 
}
{% endrequiredcode %}

{% examplecode csharp %}
calculator.Add(1, -10);

// Instead of `Arg.Is<int>(x => x < 0)`, use:
calculator.Received().Add(1, CompatArg.Is<int>(x => x < 0));

// Instead of `Arg.Any<int>()`, use:
calculator
    .Received()
    .Add(1, CompatArg.Any<int>());

// Same for Returns and DidNotReceive:
calculator.Add(CompatArg.Any<int>(), CompatArg.Is(42)).Returns(123);
calculator.DidNotReceive().Add(CompatArg.Is<int>(x => x > 10), -10);
{% endexamplecode %}

To make this a bit less verbose you may want to create a wrapper within the root of your test project with a shorter name (e.g. `ArgC`) that delegates to `CompatArg`. This will also avoid needing the extra using import, and make it easier to switch to the standard `Arg` class once you're able to update to C# 7.0 and beyond.
