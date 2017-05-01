#r @"../packages/FAKE.4.60.0/tools/FakeLib.dll"
#load @"ExtractDocs.fsx"

open Fake
open Fake.AssemblyInfoFile
open Fake.FileUtils
open System
open System.IO
open ExtractDocs
open System.Text.RegularExpressions

let buildMode = getBuildParamOrDefault "mode" "Debug"

let rootDir = "../"
let outputBaseDir = rootDir @@ "output/"
let outputDir = outputBaseDir @@ buildMode

Target "Clean" <| fun _ -> CleanDirs [ outputDir ]

Target "Default" DoNothing

// .NET Core build
Target "Restore" (fun _ ->
    DotNetCli.Restore 
        (fun p -> { p with NoCache = true })
)

Target "Build" (fun _ ->
    DotNetCli.Build
      (fun p -> 
           { p with Configuration = buildMode })
)

Target "Test" (fun _ ->
    DotNetCli.Test
      (fun p -> { p with Configuration = buildMode })
)

"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Test"
    ==> "Default"

RunTargetOrDefault "Default"
