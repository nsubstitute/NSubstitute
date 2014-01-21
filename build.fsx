#r @"ThirdParty\FAKE\FAKE.Core\tools\FakeLib.dll"
open Fake 
open System

let buildMode = getBuildParamOrDefault "buildMode" "Release"