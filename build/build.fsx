#r @"../packages/FAKE.4.60.0/tools/FakeLib.dll"

open Fake

let configuration = getBuildParamOrDefault "configuration" "Debug"

let rootDir = "../"
let outputBaseDir = rootDir @@ "output/"
let outputDir = outputBaseDir @@ configuration

Target "Clean" <| fun _ -> CleanDirs [ outputDir ]

Target "Default" DoNothing
Target "All" DoNothing

// .NET Core build
Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p -> 
        { p with 
            NoCache = true })
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
