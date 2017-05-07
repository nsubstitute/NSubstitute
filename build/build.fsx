#r @"../packages/FAKE.4.60.0/tools/FakeLib.dll"

open Fake
open System

let solutionFile  = "NSubstitute.sln"
let configuration = getBuildParamOrDefault "configuration" "Debug"

let rootDir = "../"
let outputBaseDir = rootDir @@ "output/"
let outputDir = outputBaseDir @@ configuration

Target "Default" DoNothing
Target "All" DoNothing

let vsProjProps = [ ("Configuration", configuration); ("Platform", "Any CPU") ]

Target "Clean" (fun _ ->
    !! solutionFile |> MSBuildReleaseExt "" vsProjProps "Clean" |> ignore
    CleanDirs [ outputBaseDir ]
)

Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p -> 
        { p with 
            NoCache = true })
)

Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuildReleaseExt "" vsProjProps "Rebuild"
    |> ignore
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
            Configuration = configuration })
)

"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Default"
    ==> "Package"
    ==> "All"

RunTargetOrDefault "Default"
