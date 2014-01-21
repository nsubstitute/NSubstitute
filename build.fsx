#r @"ThirdParty\FAKE\FAKE.Core\tools\FakeLib.dll"
open Fake 
open System

let ALL_TARGETS = ["NET35"; "NET40"]
let EXPERIMENTAL_TARGETS = []

let platform = getBuildParamOrDefault "platform" ALL_TARGETS.Head
let buildMode = getBuildParamOrDefault "buildMode" "Debug"
let config = String.Format("{0}-{1}", platform, buildMode)

let SOURCE_PATH = "./Source"
let OUTPUT_PATH = "./Output"

Target "Clean" (fun _ ->
     MSBuild null "Clean" ["Configuration", config] ["./Source/NSubstitute.2010.sln"]
      |> Log "Clean: "
)

Target "BuildSolution" (fun _ ->
    MSBuild null "Build" ["Configuration", config] ["./Source/NSubstitute.2010.sln"]
    |> Log "Build: "
)

Target "Default" DoNothing

"Clean"
   ==> "BuildSolution"
   ==> "Default"

RunTargetOrDefault "Default"