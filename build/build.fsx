#r @"../packages/FAKE.4.60.0/tools/FakeLib.dll"

open Fake
open System

let configuration = getBuildParamOrDefault "configuration" "Debug"

let root = __SOURCE_DIRECTORY__ </> ".."
let output = root </> "build" </> "output" </> configuration
let solutionFile  = root </> "NSubstitute.sln"

Target "Default" DoNothing
Target "All" DoNothing

let vsProjProps = [ ("Configuration", configuration); ("Platform", "Any CPU") ]

Target "Clean" (fun _ ->
    !! solutionFile |> MSBuildReleaseExt "" vsProjProps "Clean" |> ignore
    CleanDirs [ output ]
)

Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p -> p)
)

Target "Build" (fun _ ->
    DotNetCli.Build (fun p -> 
        { p with 
            Configuration = configuration })
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
