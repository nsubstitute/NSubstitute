---
title: NSubstitute.Analyzers
---

The [NSubstitute.Analyzers](https://github.com/nsubstitute/NSubstitute.Analyzers) project uses [Roslyn](https://docs.microsoft.com/en-us/visualstudio/extensibility/dotnet-compiler-platform-roslyn-extensibility) to add code analysis during compilation to detect possible errors using the NSubstitute API.

For example: as noted in the warning in [Creating a substitute](/help/creating-a-substitute/), non-virtual members of a class can not be intercepted by NSubstitute. This can cause our tests to behave unpredictably if we attempt to use `.Returns()` or `.Received()` with them. NSubstitute.Analyzers can detect attempts to substitute for non-virtual members and raise an [NS1000](https://github.com/nsubstitute/NSubstitute.Analyzers/blob/dev/documentation/rules/NS1000.md) warning at compile-time, including suggestions on how to fix it. It's much nicer to find these problems at compile-time than during test execution, or worse, not finding them until much later when we realise our tests were not doing exactly what we thought they were!

The NSubstitute NuGet package will work fine without any additional packages, but we highly recommend also adding the appropriate Analyzers package to your project (C# or VB):

* [NSubstitute.Analyzers.CSharp](https://www.nuget.org/packages/NSubstitute.Analyzers.CSharp/) for C# projects
* [NSubstitute.Analyzers.VisualBasic](https://www.nuget.org/packages/NSubstitute.Analyzers.VisualBasic/) for VB projects

## Diagnostics

See [NSubstitute.Analyzers documentation](https://github.com/nsubstitute/NSubstitute.Analyzers/tree/dev/documentation) for a list of the potential issues NSubstitute.Analyzers can help detect.

## Spelling

The Australian members of our team would like to point out they would have preferred to use the English spelling `NSubstitute.Analysers` for this project, but have begrudgingly agreed to use the American (mis)spelling instead. :)