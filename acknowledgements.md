The aim of this file is to acknowledge the software projects that have been used to create NSubstitute, particularly those distributed as Open Source Software. They have been invaluable in helping us produce this software.

# Software distributed with/compiled into NSubstitute

## Castle.Core
NSubstitute is built on the Castle.Core library, particularly Castle.DynamicProxy which is used for generating proxies for types and intercepting calls made to them so that NSubstitute can record them. 

Castle.Core is maintained by the Castle Project [https://www.castleproject.org/] and is released under the Apache License, Version 2.0 [https://www.apache.org/licenses/LICENSE-2.0.html].

# Software used to help build NSubstitute

## NUnit [https://nunit.org/]
NUnit is used for coding and running unit and integration tests for NSubstitute. It is distributed under an open source zlib/libpng based license [https://www.opensource.org/licenses/zlib-license.html].

## Rhino Mocks [https://hibernatingrhinos.com/oss/rhino-mocks]
Used for mocking parts of the NSubstitute mocking library for testing. It is distributed under the BSD license [https://www.opensource.org/licenses/bsd-license.php].

## Moq [https://github.com/moq/moq4]
Moq is not directly used in NSubstitute, but was a great source of inspiration. Moq pioneered Arrange-Act-Assert (AAA) mocking syntax for .NET, as well as removing the distinction between mocks and stubs, both of which have become important parts of NSubstitute. Moq is available under the BSD license [https://www.opensource.org/licenses/bsd-license.php].

## Jekyll [https://jekyllrb.com/]
Static website generator written in Ruby, used for NSubstitute's website and documentation. Distributed under the MIT license [https://www.opensource.org/licenses/bsd-license.php].

## SyntaxHighlighter [https://alexgorbatchev.com/SyntaxHighlighter/]
Open source, JavaScript, client-side code highlighter used for highlighting code samples on the NSubstitute website. Distributed under the MIT License [https://en.wikipedia.org/wiki/MIT_License] and the GPL [https://www.gnu.org/copyleft/lesser.html].

## FAKE [https://fsharp.github.io/FAKE/]
FAKE (F# Make) is used for NSubstitute's build. It is inspired by `make` and `rake`. FAKE is distributed under a dual Apache 2 / MS-PL license [https://github.com/fsharp/FAKE/blob/master/License.txt].

## Microsoft .NET Framework [https://www.microsoft.com/net/]
NSubstitute is coded in C# and compiled using Microsoft .NET. It can also run and compile under Mono [https://www.mono-project.com], an open source implementation of the open .NET standards for C# and the CLI.

Microsoft's .NET Framework is available under a EULA (and possibly other licenses like MS Reference Source License).
Mono is available under four open source licenses for different parts of the project (including MIT/X11, GPL, MS-Pl). These are described on the project site [https://www.mono-project.com/Licensing].

## BenchmarkDotNet [https://github.com/dotnet/BenchmarkDotNet]
Really useful tool for benchmarking .NET code! Available for use under MIT License [https://github.com/dotnet/BenchmarkDotNet/blob/master/LICENSE.md].

# Previously used for building NSubstitute

Even though they are no longer directly used for NSubstitute, the following projects were really helpful in building previous NSubstitute versions.

## Microsoft Reactive Extensions for .NET (Rx) [https://msdn.microsoft.com/en-us/devlabs/ee794896]
Used to provide .NET 3.5 with some of the neat concurrency helper classes that ship with out of the box with .NET 4.0. Distributed under a EULA [https://msdn.microsoft.com/en-us/devlabs/ff394099]. No longer required since we stopped supporting .NET 3.5.

## Microsoft Ilmerge [https://github.com/dotnet/ILMerge]
Used for combining assemblies so NSubstitute can be distributed as a single DLL. Available for use under a EULA as described on the ilmerge site. No longer used now NuGet has taken over the .NET world.

## 7-Zip [https://www.7-zip.org/]
7-zip was used to zip up NSubstitute distributions as part of the automated build process. Distributed under a mixed GNU LGPL / unRAR licence [https://www.7-zip.org/license.txt]. No longer required as we just ship NuGet packages now.

# Other acknowledgements

## Software developers
Yes, you! To everyone who has tried to get better at the craft and science of programming, especially those of you who have talked, tweeted, blogged, screencasted, and/or contributed software or ideas to the community.

No software developers were harmed to any significant extent during the production of NSubstitute, although some had to get by on reduced sleep.

