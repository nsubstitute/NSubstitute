#r @"../packages/FAKE.4.63.0/tools/FakeLib.dll"
#load @"ExtractDocs.fsx"

open Fake
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
            <TargetFrameworks>netcoreapp1.1</TargetFrameworks>
          </PropertyGroup>
          <ItemGroup>
            <PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.0.0" />
            <PackageReference Include="NUnit" Version="3.6.1" />
            <PackageReference Include="NUnit3TestAdapter" Version="3.8.0-alpha1" />
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

// List targets, similar to `rake -T`
Target "-T" <| fun _ ->
    printfn "Optional config options:"
    printfn "  configuration=Debug|Release"
    printfn "  packageVersionSuffix=alpha|beta|beta2|...   - used to tag a NuGet package as prerelease"
    printfn ""
    PrintTargets()

"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Default"
    ==> "CodeFromDocumentation"
    ==> "Package"
    ==> "All"

RunTargetOrDefault "Default"
