#r @"../packages/FAKE.4.63.0/tools/FakeLib.dll"
#load @"ExtractDocs.fsx"

open Fake
open Fake.TargetHelper
open System
open System.IO
open System.Text.RegularExpressions
open ExtractDocs

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
        log <| sprintf "Converting %s to %s" file target
        TransformFile file target (ExtractDocs.strToFixture fileName)

    let Convert paths targetDir =
        let paths = paths |> Seq.toList
        for p in paths do
            trace <| sprintf "Convert from %s to %s" p targetDir
            let files = !! "*.markdown" ++ "*.html" |> SetBaseDir p
            for file in files do
                ConvertFile file targetDir

let getVersion () =
    let tag = Git.CommandHelper.runSimpleGitCommand "" "describe --tags --long"
    let result = Regex.Match(tag, @"v(\d+)\.(\d+)\.(\d+)\-(\d+)").Groups
    let getMatch (i:int) = result.[i].Value
    sprintf "%s.%s.%s.%s" (getMatch 1) (getMatch 2) (getMatch 3) (getMatch 4)

let root = __SOURCE_DIRECTORY__ </> ".."

let configuration = getBuildParamOrDefault "configuration" "Debug"
let version = getVersion ()

let additionalArgs =
    [ sprintf "-p:Version=%s" version
    ; sprintf "-p:PackageVersion=%s" version
    ]

let output = root </> "bin" </> configuration
let solutionFile  = root </> "NSubstitute.sln"

Target "Default" DoNothing
Target "All" DoNothing

Description("Clean compilation artifacts and remove output bin directory")
Target "Clean" (fun _ ->
    let vsProjProps = [ ("Configuration", configuration); ("Platform", "Any CPU") ]
    !! solutionFile |> MSBuild "" "Clean" vsProjProps |> ignore
    CleanDirs [ output ]
)

Description("Restore dependencies")
Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p -> p)
)

Description("Compile all projects")
Target "Build" (fun _ ->
    DotNetCli.Build (fun p ->
        { p with
            Configuration = configuration
            AdditionalArgs = additionalArgs
            })
)

Description("Run tests")
Target "Test" (fun _ ->
    DotNetCli.Test (fun p ->
        { p with
            Project = "tests/NSubstitute.Acceptance.Specs/NSubstitute.Acceptance.Specs.csproj"
            Configuration = configuration })
)

Description("Generate Nuget package")
Target "Package" (fun _ ->
    DotNetCli.Pack (fun p ->
        { p with
            Configuration = configuration
            Project = "src/NSubstitute/NSubstitute.csproj"
            AdditionalArgs = additionalArgs
            })
)

Description("Run all benchmarks. Must be run with configuration=Release.")
Target "Benchmarks" (fun _ ->
    if configuration <> "Release" then
        failwith "Benchmarks can only be run in Release mode. Please re-run the build in Release configuration."

    let benchmarkCsproj = "tests/NSubstitute.Benchmarks/NSubstitute.Benchmarks.csproj" |> Path.GetFullPath
    let benchmarkToRun = getBuildParamOrDefault "benchmark" "*" // Defaults to "*" (all)
    [ "netcoreapp1.1" ]
    |> List.iter (fun framework ->
        traceImportant ("Benchmarking " + framework)
        let work = output </> "benchmark-" + framework
        ensureDirectory work
        DotNetCli.RunCommand
            (fun p -> { p with WorkingDir = work; TimeOut = TimeSpan.FromHours 2. })
            ("run --framework " + framework + " --project " + benchmarkCsproj + " -- " + benchmarkToRun)
    )
)

Description("Extract, build and test code from documentation.")
Target "TestCodeFromDocs" <| fun _ ->
    let outputCodePath = output </> "CodeFromDocs"
    CreateDir outputCodePath
    // generate samples from docs
    ExamplesToCode.Convert [ "./docs/"; "./docs/help/_posts/"; "./" ] outputCodePath
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
    DotNetCli.Restore (fun p -> { p with Project = projPath })
    DotNetCli.Build (fun p -> { p with Project = projPath })
    DotNetCli.Test (fun p -> { p with Project = projPath })

let tryFindFileOnPath (file : string) : string option =
    Environment.GetEnvironmentVariable("PATH").Split([| Path.PathSeparator |])
    |> Seq.append ["."]
    |> fun path -> tryFindFile path file

Description("Build documentation website. Requires Ruby, bundler and jekyll.")
Target "Documentation" <| fun _ -> 
    log "building site..."
    let exe = [ "bundle.bat"; "bundle" ]
                |> Seq.map tryFindFileOnPath
                |> Seq.collect (Option.toList)
                |> Seq.tryFind (fun _ -> true)
                |> function | Some x -> log ("using " + x); x
                            | None   -> log ("count not find exe"); "bundle"

    let workingDir = "./docs/"
    let docOutputRelativeToWorkingDir = ".." </> output </> "nsubstitute.github.com"
    let result = 
        ExecProcess (fun info -> 
                        info.UseShellExecute <- false
                        info.CreateNoWindow <- true
                        info.FileName <- exe
                        info.WorkingDirectory <- workingDir
                        info.Arguments <- "exec jekyll \"" + docOutputRelativeToWorkingDir + "\"")
                    (TimeSpan.FromMinutes 5.)
    if result = 0 then
        "site built in " + docOutputRelativeToWorkingDir |> log
    else
        "failed to build site" |> failwith

Description("List targets, similar to `rake -T`. For more details, run `--listTargets` instead.")
Target "-T" <| fun _ ->
    printfn "Optional config options:"
    printfn "  configuration=Debug|Release"
    printfn "  benchmark=*|<benchmark name>  (only for Benchmarks target in Release mode)"
    printfn ""
    PrintTargets()

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

RunTargetOrDefault "Default"
