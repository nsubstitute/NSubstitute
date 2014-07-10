#### 3.2.0 - 07.07.2014
* BREAKING CHANGE: API for CreateAssemblyInfoWithConfig was set back to original version
    This resets the breacking change introduced in https://github.com/fsharp/FAKE/pull/471

#### 3.1.2 - 07.07.2014
* Automatic tool search for SpecFlowHelper - https://github.com/fsharp/FAKE/pull/496

#### 3.1.1 - 05.07.2014
* GuardedAwaitObservable was made public by accident - this was fixed
* Add support for remote service admin - https://github.com/fsharp/FAKE/pull/492

#### 3.1.0 - 04.07.2014
* New FSC helper allows to call F# compiler directly from FAKE - https://github.com/fsharp/FAKE/pull/485

#### 3.0.8 - 02.07.2014
* "CustomDictionary" support for FxCop - https://github.com/fsharp/FAKE/pull/489

#### 3.0.7 - 01.07.2014
* Check if file exists before delete in AssemblyInfoFile

#### 3.0.6 - 01.07.2014
* Use FSharp.Compiler.Service 0.0.58

#### 3.0.5 - 01.07.2014
* Report all targets if a target error occurs

#### 3.0.4 - 01.07.2014
* Use FSharp.Compiler.Service with better FSharp.Core resolution - https://github.com/fsharp/FSharp.Compiler.Service/issues/156

#### 3.0.3 - 01.07.2014
* Don't break in MSBuildHelper
* Put FSharp.Core.optdata and FSharp.Core.sigdata into nuget package

#### 3.0.2 - 01.07.2014
* Fixed TargetTracing

#### 3.0.1 - 29.06.2014
* Fixed SourceLinking of FAKE
* Added new exception trap for Fsi creation
* -br in command line will run debugger in F# scripts - https://github.com/fsharp/FAKE/pull/483
* Null check in NuGet helper - https://github.com/fsharp/FAKE/pull/482

#### 3.0.0 - 27.06.2014
* Use FSharp.Compiler.Service 0.0.57 instead of fsi.exe
* Better error message for registry access
* Fall back to 32bit registry keys if 64bit cannot be found
* Improved SqlServer Disconnect error message
* Log "kill all processes" only when needed
* Try to run as x86 due to Dynamics NAV problems
* Allow to use /gac for FxCop
* Make NuGet description fit into single line

#### 2.18.2 - 17.06.2014
* Use Nuget.Core 2.8.2
* Fix NUnitProcessModel.SeparateProcessModel - https://github.com/fsharp/FAKE/pull/474

#### 2.18.1 - 16.06.2014
* Improved CLI documentation - https://github.com/fsharp/FAKE/pull/472
* Added Visual Basic support to AssemblyFileInfo task and make Namespace optional in config - https://github.com/fsharp/FAKE/pull/471
* Added support for OctoTools ignoreExisting flag - https://github.com/fsharp/FAKE/pull/470
* OctoTools samples fixed - https://github.com/fsharp/FAKE/pull/468 https://github.com/fsharp/FAKE/pull/469
* Added support for FxCop /ignoregeneratedcode parameter - https://github.com/fsharp/FAKE/pull/467
* CreateAssemblyInfo works with nonexisting directories - https://github.com/fsharp/FAKE/pull/466

#### 2.18.0 - 11.06.2014
* New (backwards compat) CLI for FAKE that includes FSI cmd args passing - https://github.com/fsharp/FAKE/pull/455
* New updateApplicationSetting method - https://github.com/fsharp/FAKE/pull/462
* Support for msbuild /noconlog - https://github.com/fsharp/FAKE/pull/463
* RoundhouseHelper - https://github.com/fsharp/FAKE/pull/456

#### 2.17.9 - 03.06.2014
* Pass optional arguments to deployment scripts
* Support building source packages without project file

#### 2.17.8 - 30.05.2014
* Display messages when deploy fails
* Fix formatting in FAKE.Deploy docs
* Fix memory usage in FAKE.Deploy

#### 2.17.7 - 28.05.2014
* Increase WebClient's request timeout to 20 minutes - https://github.com/fsharp/FAKE/pull/442

#### 2.17.6 - 27.05.2014
* Mainly Layout fixes and disabling authenticate in FAKE.Deploy https://github.com/fsharp/FAKE/pull/441

#### 2.17.5 - 26.05.2014
* Deploy PDBs via nuget https://github.com/fsharp/FAKE/issues/435 

#### 2.17.4 - 25.05.2014
* Release Notes parser should not drop asterisk at end of lines
* Corrected location of @files@ in nuspec sample

#### 2.17.3 - 23.05.2014
* Allow to report tests to AppVeyor

#### 2.17.2 - 23.05.2014
* fix appveyor msbuild logger

#### 2.17.1 - 23.05.2014
* Don't add Teamcity logger if not needed

#### 2.17.0 - 23.05.2014
* Fake.Deploy agent requires user authentication
* Remove AutoOpen von AppVeyor

#### 2.16.3 - 23.05.2014
* fix order of arguments in call to CopyFile
 
#### 2.16.2 - 22.05.2014
* Support MSTest test settings - https://github.com/fsharp/FAKE/pull/428

#### 2.16.1 - 21.05.2014
* If the NAV error file contains no compile errors return the length

#### 2.16.0 - 21.05.2014
* Promoted the master branch as default branch and removed develop branch

#### 2.15.7 - 21.05.2014
* Remove AutoOpen from TaskRunnerHelper
* Adding Metadata to AsssemblyInfo
* Analyze the Dynamics NAV log file and report the real error count

#### 2.15.6 - 14.05.2014
* Allow to retrieve version no. from assemblies

#### 2.15.5 - 08.05.2014
* Fix issue with symbol packages in NugetHelper

#### 2.15.4 - 24.4.2014
* Fix issues in the ProcessHelper - https://github.com/fsharp/FAKE/pull/412 and https://github.com/fsharp/FAKE/pull/411

#### 2.15.2 - 24.4.2014
* Allow to register BuildFailureTargets - https://github.com/fsharp/FAKE/issues/407
* UnionConverter no longer needed for Json.Net

#### 2.14.13 - 24.04.2014
* Handle problems with ProgramFilesX86 on mono - https://github.com/tpetricek/FsLab/pull/32

#### 2.14.12 - 24.04.2014
* Change the MSBuild 12.0 path settings according to https://github.com/tpetricek/FsLab/pull/32

#### 2.14.1 - 23.04.2014
* Silent mode for MSIHelper - https://github.com/fsharp/FAKE/issues/400

#### 2.14.0 - 22.04.2014
* Support for OpenCover - https://github.com/fsharp/FAKE/pull/398
* Support for ReportsGenerator - https://github.com/fsharp/FAKE/pull/399

#### 2.13.4 - 14.04.2014
* Adding AppVeyor environment variables 
* New BulkReplaceAssemblyInfoVersions task - https://github.com/fsharp/FAKE/pull/394
* Fixed default nuspec file
* "Getting started" tutorial uses better folder structure

#### 2.13.3 - 09.04.2014
* Allows explicit file specification on the NuGetParams Type

#### 2.13.2 - 07.04.2014
* Fix TypeScript output dir
* Add better docs for the TypeScript compiler.

#### 2.13.1 - 05.04.2014
* Don't call the TypeScript compiler more than once
* New parameters for TypeScript

#### 2.13.0 - 04.04.2014
* Enumerate the files lazily in the File|Directory active pattern
* Using Nuget 2.8.1

#### 2.13.0-alpha2 - 03.04.2014
* Added TypeScript 1.0 support

#### 2.13.0-alpha1 - 02.04.2014
* Added TypeScript support

#### 2.12.2 - 31.03.2014
* Fixed ProcessTestRunner

#### 2.12.1-alpha3 - 31.03.2014
* Fixed mono build on Travis

#### 2.12.0 - 31.03.2014
* Add getDependencies to NugetHelper
* SourceLink support
* NancyFx instead of ASP.NET MVC for Fake.Deploy
* Allows to execute processes as unit tests.
* Adding SourceLinks
* Move release management back to the local machine (using this document)
* Allow to run MsTest test in isolation
* Fixed Nuget.packSymbols
* Fixed bug in SemVer parser
* New title property in Nuspec parameters
* Added option to disabled FAKE's automatic process killing
* Better AppyVeyor integration
* Added ability to define custom MSBuild loggers
* Fix for getting the branch name with Git >= 1.9
* Added functions to write and delete from registry
* NUnit NoThread, Domain and StopOnError parameters
* Add support for VS2013 MSTest
* Lots of small fixes

#### 2.2

* Created new packages on nuget:
	* Fake.Deploy - allows to use FAKE scripts in deployment.
	* Fake.Experimental - new stuff where we aren't sure if we want to support it.
	* Fake.Gallio - contains the Gallio runner support.
	* Fake.SQL - Contains tasks for SQL Server.
	* Fake.Core - All the basic features and FAKE.exe.
* Created documentation and tutorials - see http://fsharp.github.io/FAKE/
* New tasks:
	* Added ReleaseNotes parser
	* Added Dynamics NAV helper
	* Added support for MSTest and fixie
	* Parallel NUnit task
	* New AssemblyInfoFile task
	* Support for Octopus Deploy
	* Support for MAGE
	* Suppport for Xamarin's xpkg
	* Many other new tasks
* Fake.Boot
* New Globbing system
* Tons of bug fixes
* Bundles F# 3.0 compiler and FSI.

#### 1.72.0.0

* "RestorePackages" allows to restore nuget packages

#### 1.70.0.0

* FAKE nuget package comes bundles with a fsi.exe
* Self build downloads latest FAKE master via nuget

#### 1.66.1.0

* Fixed bug where FAKE.Deploy didn't run the deploy scripts where used as a windows service
* It's possible to add file loggers for MSBuild
* Fixed path resolution for fsi on *nix
* BREAKING CHANGE: Removed version normalization from NuGet package creation
* Fixes for NUNit compatibility on mono 
* Fixes in ProcessHelper for mono compatibility
* Fixes in the mono build
* Improved error reporting in Fake.exe
* Added a SpecFlow helper
* Fixed some issues in file helper routines when working with no existing directory chain

#### 1.64.1.0

* Fixed bug where FAKE didn't run the correct build script

#### 1.64.0.0

* New conditional dependency operator =?>
* BREAKING CHANGE: Some AssemblyInfo task parameters are now option types. See type hints.

#### 1.62.0.0

* New RegAsm task, allows to create TLBs from a dll.
* New MSI task, allows to install or uninstall msi files.
* StringHelper.NormalizeVersion fixed for WiX.

#### 1.58.9.0

* Allow to choose specific nunit-console runner.

#### 1.58.6.0

* Using nuget packages for mspec.
* FAKE tries to kill all MSBuild and FSI processes at the end of a build.

#### 1.58.1.0

* Removed message system for build output. Back to simpler tracing.

#### 1.58.0.0

* ReplaceAssemblyInfoVersions task allows to replace version info in AssemblyVersion-files
* New task ConvertFileToWindowsLineBreaks

#### 1.56.10.0

* Allows to build .sln files

#### 1.56.0.0

* Allows to publish symbols via nuget.exe
* Autotrim trailing .0 from version in order to fullfill nuget standards.

#### 1.54.0.0

* If the publishment of a Nuget package fails, then FAKE will try it again.
* Added Changelog.markdown to FAKE deployment
* Added RequireExactly helper function in order to require a specific nuget dependency.
* NugetHelper.GetPackageVersion - Gets the version no. for a given package in the packages folder.
* EnvironmentHelper.getTargetPlatformDir - Gets the directory for the given target platform.

#### 1.52.0.0

* Some smaller bugfixes
* New dependency syntax with ==> and <=>
* Tracing of StackTrace only if TargetHelper.PrintStackTraceOnError was set to true

#### 1.50.0.0

* New task DeleteDirs allows to delete multiple directories.
* New parameter for NuGet dependencies.

#### 1.48.0.0

* Bundled with docu.exe compiled against .Net 4.0.
* Fixed docu calls to run with full filenames.
* Added targetplatform, target and log switches for ILMerge task.
* Added Git.Information.getLastTag() which gets the last git tag by calling git describe.
* Added Git.Information.getCurrentHash() which gets the last current sha1.

#### 1.46.0.0

* Fixed Nuget support and allows automatic push.

#### 1.44.0.0

* Tracing of all external process starts.
* MSpec support.
