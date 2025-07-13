---
layout: landing
title: A friendly substitute for .NET mocking libraries
---

<div id="downloads" class="sidebar download">

# Welcome to NSubstitute

<p>Mock, stub, fake, spy, test double? Strict or loose? Nah, just substitute for the type you need!</p>
<p>NSubstitute is designed for Arrange-Act-Assert (AAA) testing, so you just need to arrange how it should work, then assert it received the calls you expected once you're done.</p>

<ul>
<li class="nuget">
<a href="https://nuget.org/List/Packages/NSubstitute">Install via NuGet:</a> <code>Install-Package NSubstitute</code>
</li>
<li class="nuget"><a href="/help/nsubstitute-analysers">Optional analysers for C#:</a>
<code>Install-Package NSubstitute.<wbr>Analyzers.<wbr>CSharp</code>
</li>
<li class="nuget"><a href="/help/nsubstitute-analysers">Optional analysers for VB:</a>
<code>Install-Package NSubstitute.<wbr>Analyzers.<wbr>VisualBasic</code>
</li>
<li class="github">
<a href="https://github.com/nsubstitute/nsubstitute">Source</a>
</li>
</ul>
</div>

<div id="features">
<div class="feature" markdown="1">

## Simple, succinct, pleasant to use

```csharp
//Create:
var calculator = Substitute.For<ICalculator>();

//Set a return value:
calculator.Add(1, 2).Returns(3);
Assert.That(calculator.Add(1, 2), Is.EqualTo(3));

//Check received calls:
calculator.Received().Add(1, Arg.Any<int>());
calculator.DidNotReceive().Add(2, 2);

//Raise events
calculator.PoweringUp += Raise.Event();
```

<!--
```requiredcode
    public interface ICalculator
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event EventHandler PoweringUp;
    }
```
-->
</div>

<div class="feature" markdown="1">

## Helpful exceptions

<div class="highlight">

```text
ReceivedCallsException : Expected to receive a call matching:
    Add(1, 2)
Actually received no matching calls.
Received 2 non-matching calls (non-matching arguments indicated with '*' characters):
    Add(*4*, *7*)
    Add(1, *5*)
```

</div>
</div>

</div>

<div class="sidebar">
<div id="why-use-it" markdown="1">

### Another library?

<p>There are already some great mocking libraries around for .NET, so why create another? We found that for all their great features, none of the existing libraries had the succinct syntax we were craving &mdash; the code required to configure test doubles quickly obscured the intention behind our tests.</p>

<p>We've attempted to make the most frequently required operations obvious and easy to use, keeping less usual scenarios discoverable and accessible, and all the while maintaining as much natural language as possible.</p>

<p>Perfect for those new to testing, and for others who would just like to to get their tests written with less noise and fewer lambdas.</p>

</div>
</div>
