#r @"../packages/FAKE.4.60.0/tools/FakeLib.dll"

open Fake

let buildMode = getBuildParamOrDefault "mode" "Debug"

let rootDir = "../"
let outputBaseDir = rootDir @@ "output/"
let outputDir = outputBaseDir @@ buildMode

Target "Clean" <| fun _ -> CleanDirs [ outputDir ]

Target "Default" DoNothing

// .NET Core build
Target "Restore" (fun _ ->
    DotNetCli.Restore (fun p -> { p with NoCache = true })
)

Target "Build" (fun _ ->
    DotNetCli.Build
      (fun p -> { p with Configuration = buildMode })
)

Target "Test" (fun _ ->
    DotNetCli.Test (fun p -> 
        { p with 
            Project = "tests/NSubstitute.Acceptance.Specs/NSubstitute.Acceptance.Specs.csproj"
            Configuration = buildMode })
)

"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Default"

RunTargetOrDefault "Default"
