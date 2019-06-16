#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet
nuget Fake.DotNet.Cli
nuget Fake.Tools.Git
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"
#load @"ExtractDocs.fsx"
// Workaround to make Intellisense work, see https://github.com/fsharp/FAKE/issues/1938
#if !FAKE
  #r "netstandard"
#endif

open System
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators
open Fake.Tools

open ExtractDocs

let target = Target.create
let description = Target.description

module FileReaderWriter =
    let Read file = File.ReadAllText(file)
    let Write file text = File.WriteAllText(file, text)
    let TransformFile file target (f : string -> string) =
        Read file
        |> f
        |> Write target

module ExamplesToCode =
    open FileReaderWriter

    let ConvertFile file targetDir =
        let fileName = Path.GetFileNameWithoutExtension(file)
        let target = targetDir @@ fileName + ".cs"
        Trace.log <| sprintf "Converting %s to %s" file target
        TransformFile file target (ExtractDocs.strToFixture fileName)

    let Convert paths targetDir =
        let paths = paths |> Seq.toList
        for p in paths do
            Trace.trace <| sprintf "Convert from %s to %s" p targetDir
            let files = !! "*.markdown" ++ "*.html" ++ "*.md" |> GlobbingPattern.setBaseDir p
            for file in files do
                ConvertFile file targetDir

type BuildVersion = { assembly: string; file: string; info: string; package: string }
let getVersion () =
    // The --first-parent flag is needed to make our walk linear from current commit and top.
    // This way also merge commit is counted as "1".
    let desc = Git.CommandHelper.runSimpleGitCommand "" "describe --tags --long --abbrev=40 --first-parent --match=v*"
    let result = Regex.Match(desc,
                             @"^v(?<maj>\d+)\.(?<min>\d+)\.(?<rev>\d+)(?<pre>-\w+\d*)?-(?<num>\d+)-g(?<sha>[a-z0-9]+)$",
                             RegexOptions.IgnoreCase)
                      .Groups
    let getMatch (name:string) = result.[name].Value

    let (major, minor, revision, preReleaseSuffix, commitsNum, commitSha) =
        (getMatch "maj" |> int, getMatch "min" |> int, getMatch "rev" |> int, getMatch "pre", getMatch "num" |> int, getMatch "sha")

    // Assembly version should contain major and minor only, as no breaking changes are expected in bug fix releases.
    let assemblyVersion = sprintf "%d.%d.0.0" major minor
    let fileVersion = sprintf "%d.%d.%d.%d" major minor revision commitsNum
 
    // If number of commits since last tag is greater than zero, we append another identifier with number of commits.
    // The produced version is larger than the last tag version.
    // If we are on a tag, we use version without modification.
    // Examples of output: 3.50.2.1, 3.50.2.215, 3.50.1-rc1.3, 3.50.1-rc3.35
    let packageVersion = match commitsNum with
                         | 0 -> sprintf "%d.%d.%d%s" major minor revision preReleaseSuffix
                         | _ -> sprintf "%d.%d.%d%s.%d" major minor revision preReleaseSuffix commitsNum

    let infoVersion = match commitsNum with
                      | 0 -> packageVersion
                      | _ -> sprintf "%s-%s" packageVersion commitSha

    { assembly = assemblyVersion; file = fileVersion; info = infoVersion; package = packageVersion }
 
let root = __SOURCE_DIRECTORY__ </> ".." |> Path.getFullName

let configuration = Environment.environVarOrDefault "configuration" "Debug"
let version = getVersion ()

let additionalArgs = [
    "AssemblyVersion", version.assembly
    "FileVersion", version.file
    "InformationalVersion", version.info
    "PackageVersion", version.package
]

let output = root </> "bin" </> configuration

target "Default" ignore
target "All" ignore

description("Clean compilation artifacts and remove output bin directory")
target "Clean" (fun _ ->
    DotNet.exec (fun p -> { p with WorkingDirectory = root }) "clean"
        (sprintf "--configuration %s --verbosity minimal" configuration)
        |> ignore
    Shell.cleanDirs [ output ]
)

description("Restore dependencies")
target "Restore" (fun _ ->
    DotNet.restore (fun p -> p) "NSubstitute.sln"
)

description("Compile all projects")
target "Build" (fun _ ->
    DotNet.build (fun p -> { p with Configuration = DotNet.BuildConfiguration.fromString configuration
                                    MSBuildParams = { p.MSBuildParams with Properties = additionalArgs }}) 
                                    "NSubstitute.sln"
)

description("Run tests")
target "Test" (fun _ ->
    DotNet.test (fun p -> { p with Configuration = DotNet.BuildConfiguration.fromString configuration
                                   MSBuildParams = { p.MSBuildParams with Properties = additionalArgs }}) 
                                   "tests/NSubstitute.Acceptance.Specs/NSubstitute.Acceptance.Specs.csproj"
)

description("Generate Nuget package")
target "Package" (fun _ ->
    DotNet.pack (fun p -> { p with Configuration = DotNet.BuildConfiguration.fromString configuration
                                   MSBuildParams = { p.MSBuildParams with Properties = additionalArgs }}) 
                                   "src/NSubstitute/NSubstitute.csproj"
)

description("Run all benchmarks. Must be run with configuration=Release.")
target "Benchmarks" (fun _ ->
    if configuration <> "Release" then
        failwith "Benchmarks can only be run in Release mode. Please re-run the build in Release configuration."

    let benchmarkCsproj = root </> "tests/NSubstitute.Benchmarks/NSubstitute.Benchmarks.csproj" |> Path.getFullName
    let benchmarkToRun = Environment.environVarOrDefault "benchmark" "*" // Defaults to "*" (all)
    [ "netcoreapp2.1" ]
    |> List.iter (fun framework ->
        Trace.traceImportant ("Benchmarking " + framework)
        let work = output </> "benchmark-" + framework
        Directory.ensure work
        DotNet.exec (fun p -> { p with WorkingDirectory = work }) "run"
            ("--framework " + framework + " --project " + benchmarkCsproj + " -- " + benchmarkToRun)
            |> ignore
    )
)

description("Extract, build and test code from documentation.")
target "TestCodeFromDocs" <| fun _ ->
    let outputCodePath = output </> "CodeFromDocs"
    Directory.create outputCodePath
    // generate samples from docs
    ExamplesToCode.Convert [ root </> "docs/"; root </> "docs/help/_posts/"; root ] outputCodePath
    // compile code samples
    let csproj = """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFrameworks>netcoreapp1.1;net46</TargetFrameworks>
          </PropertyGroup>
          <ItemGroup>
            <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.3.0" />
            <PackageReference Include="NUnit" Version="3.8.1" />
            <PackageReference Include="NUnit3TestAdapter" Version="3.8.0" />
          </ItemGroup>
          <ItemGroup>
            <ProjectReference Include="..\..\..\src\NSubstitute\NSubstitute.csproj" />
          </ItemGroup>
          <ItemGroup>
            <Service Include="{82a7f48d-3b50-4b1e-b82e-3ada8210c358}" />
          </ItemGroup>
    </Project>
    """
    let projPath = outputCodePath </> "Docs.csproj"
    FileReaderWriter.Write projPath csproj
    DotNet.restore (fun p -> p) projPath
    DotNet.build (fun p -> p) projPath
    DotNet.test (fun p -> p) projPath

let tryFindFileOnPath (file : string) : string option =
    Environment.GetEnvironmentVariable("PATH").Split([| Path.PathSeparator |])
    |> Seq.append ["."]
    |> fun path -> ProcessUtils.tryFindFile path file

description("Build documentation website. Requires Ruby, bundler and jekyll.")
target "Documentation" <| fun _ -> 
    Trace.log "Building site..."
    let exe = [ "bundle.bat"; "bundle" ]
                |> Seq.map tryFindFileOnPath
                |> Seq.collect (Option.toList)
                |> Seq.tryFind (fun _ -> true)
                |> function | Some x -> Trace.log ("using " + x); x
                            | None   -> Trace.log ("count not find exe"); "bundle"

    let workingDir = root </> "docs/"
    let docOutputRelativeToWorkingDir = ".." </> output </> "nsubstitute.github.com"
    
    // TODO migrate the following to FAKE API: CreateProcess.ofStartInfo(p)
    // https://fake.build/apidocs/v5/fake-core-createprocess.html
    // that doesn't work for some reason
    let p = ProcessStartInfo(
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = exe,
                    WorkingDirectory = workingDir,
                    Arguments = "exec jekyll build -d \"" + docOutputRelativeToWorkingDir + "\"")
    let proc = Process.Start(p)
    proc.WaitForExit()
    let result = proc.ExitCode
    if result = 0 then
        "Site built in " + docOutputRelativeToWorkingDir |> Trace.log
    else
        "failed to build site" |> failwith

description("List targets, similar to `rake -T`. For more details, run `--listTargets` instead.")
target "-T" <| fun _ ->
    printfn "Optional config options:"
    printfn "  configuration=Debug|Release"
    printfn "  benchmark=*|<benchmark name>  (only for Benchmarks target in Release mode)"
    printfn ""
    Target.listAvailable()

"Clean" ?=> "Build"
"Clean" ?=> "Test"
"Clean" ?=> "Restore"
"Clean" ?=> "Documentation"
"Clean" ?=> "TestCodeFromDocs"
"Clean" ?=> "Package"
"Clean" ?=> "Default"

"Build"         <== [ "Restore" ]
"Test"          <== [ "Build" ]
"Documentation" <== [ "TestCodeFromDocs" ]
"Benchmarks"     <== [ "Build" ]
// For packaging, use a clean build and make sure all tests (inc. docs) pass.
"Package"       <== [ "Clean"; "Build"; "Test"; "TestCodeFromDocs" ]

"Default"       <== [ "Restore"; "Build"; "Test" ]
"All"           <== [ "Clean"; "Default"; "Documentation"; "Package" ]

Target.runOrDefault "Default"
