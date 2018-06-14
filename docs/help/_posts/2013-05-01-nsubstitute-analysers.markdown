---
title: NSubstitute.Analyzers
layout: post
---

The [NSubstitute.Analyzers](https://github.com/nsubstitute/NSubstitute.Analyzers) project uses [Roslyn](https://docs.microsoft.com/en-us/visualstudio/extensibility/dotnet-compiler-platform-roslyn-extensibility) to add code analysis during compilation to detect possible errors using the NSubstitute API.

The NSubstitute NuGet package will work fine without any additional packages, but we highly recommend also adding the appropriate Analyzers package to your project (C# or VB):

* [NSubstitute.Analyzers.CSharp](https://www.nuget.org/packages/NSubstitute.Analyzers.CSharp/) for C# projects
* [NSubstitute.Analyzers.VisualBasic](https://www.nuget.org/packages/NSubstitute.Analyzers.VisualBasic/) for VB projects

## Diagnostics

NSubstitute.Analyzers can help detect the following issues.

### NS001: NonVirtualSetupSpecification

This error occurs when attempting to substitute for a non-virtual class member. As noted in the warning in [Creating a substitute](/help/creating-a-substitute/), non-virtual members of a class can not be intercepted by NSubstitute which can cause your tests to behave unpredictably.

NSubstitute.Analyzers will pick up these cases and report a compiler error. To fix these cases, make the member virtual. If this is not possible then another options is to wrap the underlying class in an interface and substitute that instead. Either way, this is much more preferable than having a test pass or fail in a confusing way!

### NS002: UnusedReceived

This error occurs if there is a `Received()` call without a following call to assert on. This should only every occur by accident, and is easily fixed by removing the incomplete assertion, or updating it to include the expected call:

    // Error:
    sub.Received(); // NS002: UnusedReceived

    // Fixed:
    sub.Received().TheExpectedCall();

## Spelling

The Australian members of our team would like to point out they would have preferred to use the English spelling `NSubstitute.Analysers` for this project, but have begrudgingly agreed to use the American (mis)spelling instead. :)
