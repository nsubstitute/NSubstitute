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
    CleanDirs [ OUTPUT_PATH ]
)

Target "BuildSolution" (fun _ ->
    MSBuild null "Build" ["Configuration", config] ["./Source/NSubstitute.2010.sln"]
    |> Log "Build: "
)

let testDir = String.Format("{0}/{1}/{2}/", OUTPUT_PATH, buildMode, platform)
let testDlls = !! (testDir + "/**/*Specs.dll")

Target "Test" (fun _ ->
testDlls
        |>  NUnit (fun p ->
            {p with
                DisableShadowCopy = true;
                Framework = "net-4.0";
                ExcludeCategory = "Pending";
                OutputFile = testDir + "TestResults.xml"}) // TODO: different file name based on path
)

Target "Default" DoNothing

"Clean"
   ==> "BuildSolution"
   ==> "Test"
   ==> "Default"

RunTargetOrDefault "Default"