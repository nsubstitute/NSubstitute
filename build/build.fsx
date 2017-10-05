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
    let result = Regex.Match(tag, @"(v|alpha|beta|rc)(\d+)\.(\d+)\.(\d+)\-(\d+)").Groups
    let getMatch (i:int) = result.[i].Value
    (sprintf "%s.%s.%s.%s" (getMatch 2) (getMatch 3) (getMatch 4) (getMatch 5), match getMatch 1 with "v" -> "" | suffix -> suffix)

let root = __SOURCE_DIRECTORY__ </> ".."

let configuration = getBuildParamOrDefault "configuration" "Debug"
let releaseNotes = ReadFile (root </> "CHANGELOG.txt") |> ReleaseNotesHelper.parseReleaseNotes
let version, suffix = getVersion ()
let packageVersionSuffix = getBuildParamOrDefault "packageVersionSuffix" suffix // Use to tag a NuGet build as alpha/beta

let additionalArgs =
    [ sprintf "-p:Version=%s" version
    ; sprintf "-p:PackageVersion=%s" version
    ]

let output = root </> "bin" </> configuration
let solutionFile  = root </> "NSubstitute.sln"

Target "Default" DoNothing
Target "All" DoNothing

Target "Clean" (fun _ ->
    let vsProjProps = [ ("Configuration", configuration); ("Platform", "Any CPU") ]
    !! solutionFile |> MSBuildReleaseExt "" vsProjProps "Clean" |> ignore
    CleanDirs [ output ]
)

Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p -> p)
)

Target "Build" (fun _ ->
    DotNetCli.Build (fun p ->
        { p with
            Configuration = configuration
            AdditionalArgs = additionalArgs
            })
)

Target "Test" (fun _ ->
    DotNetCli.Test (fun p ->
        { p with
            Project = "tests/NSubstitute.Acceptance.Specs/NSubstitute.Acceptance.Specs.csproj"
            Configuration = configuration })
)

Target "Package" (fun _ ->
    DotNetCli.Pack (fun p ->
        { p with
            Configuration = configuration
            Project = "src/NSubstitute/NSubstitute.csproj"
            VersionSuffix = packageVersionSuffix
            AdditionalArgs = additionalArgs
            })
)

Target "CodeFromDocumentation" <| fun _ ->
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

// List targets, similar to `rake -T`
Target "-T" <| fun _ ->
    printfn "Optional config options:"
    printfn "  configuration=Debug|Release"
    printfn "  packageVersionSuffix=alpha|beta|beta2|...   - used to tag a NuGet package as prerelease"
    printfn ""
    PrintTargets()

"Clean" ?=> "Build"
"Clean" ?=> "Test"
"Clean" ?=> "Restore"
"Clean" ?=> "Documentation"
"Clean" ?=> "CodeFromDocumentation"
"Clean" ?=> "Package"
"Clean" ?=> "Default"

"Build" <== [ "Restore" ]
"Test" <== [ "Build" ]
"Package" <== [ "Build"; "Test" ]
"Documentation" <== [ "CodeFromDocumentation" ]
"Default" <== [ "Restore"; "Build"; "Test" ]

"All" <== [ "Clean"; "Default"; "Documentation"; "Package" ]

RunTargetOrDefault "Default"
