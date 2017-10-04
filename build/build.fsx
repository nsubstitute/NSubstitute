#r @"../packages/FAKE.4.63.0/tools/FakeLib.dll"

open Fake
open System
open System.Text.RegularExpressions

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

"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Default"
    ==> "Package"
    ==> "All"

RunTargetOrDefault "Default"
