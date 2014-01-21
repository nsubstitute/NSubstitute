#r @"ThirdParty\FAKE\FAKE.Core\tools\FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open System

let EXPERIMENTAL_TARGETS = []

let version = "1.8.0.0"

let buildMode = getBuildParamOrDefault "buildMode" "Debug"

let OUTPUT_PATH = "./Output"

Target "Clean" (fun _ ->
    CleanDirs [ OUTPUT_PATH ]
)

Target "Version" (fun _ ->
    CreateCSharpAssemblyInfo "Source/NSubstitute/Properties/AssemblyInfo.cs"
        [Attribute.Title "NSubstitute"
         Attribute.Description "A simple substitute for .NET mocking frameworks."
         Attribute.Guid "f1571463-8354-493c-b67c-cd9cec9adf78"
         Attribute.Product "NSubstitute"
         Attribute.Copyright @"Copyright \u00A9  2009 NSubstitute Team"
         Attribute.Version version
         Attribute.FileVersion version]
)

Target "BuildSolution" (fun _ ->
  let seqBuild = Seq.map (fun config -> 
      MSBuild null "Build" ["Configuration", config] ["./Source/NSubstitute.2010.sln"]
          |> Log "Build: " )

  Seq.last (seqBuild [ "NET35-"+buildMode ; "NET40-"+buildMode ])
)

let outputDir = String.Format("{0}/{1}/", OUTPUT_PATH, buildMode)
let testDlls = !! (outputDir + "**/*Specs.dll")

Target "Test" (fun _ ->
    testDlls
        |>  NUnit (fun p ->
            {p with
                DisableShadowCopy = true;
                Framework = "net-4.0";
                ExcludeCategory = "Pending";
                OutputFile = outputDir + "TestResults.xml"}) // TODO: different file name based on path
)

Target "Default" DoNothing

let outputBasePath =  String.Format("{0}/{1}/", OUTPUT_PATH, buildMode);
let workingDir = String.Format("{0}package/", outputBasePath)

let net35binary = String.Format("{0}NET35/NSubstitute/NSubstitute.dll", outputDir)
let net40binary = String.Format("{0}NET40/NSubstitute/NSubstitute.dll", outputDir)
let net35binariesDir = String.Format("{0}lib/net35", workingDir)
let net40binariesDir = String.Format("{0}lib/net40", workingDir)

Target "NuGet" (fun _ ->
    //CreateDir workingDir
    CreateDir net35binariesDir
    CreateDir net40binariesDir

    // Copy binaries into lib path
    CopyFile net35binariesDir net35binary
    CopyFile net40binariesDir net40binary

    NuGet (fun p ->
        {p with
            OutputPath = outputBasePath
            WorkingDir = workingDir
            Version = version
             }) "Build/NSubstitute.nuspec"
)


Target "Zip" (fun _ ->
    //CreateDir workingDir
    CreateDir net35binariesDir
    CreateDir net40binariesDir

    // Copy binaries into lib path
    CopyFile net35binariesDir net35binary
    CopyFile net40binariesDir net40binary
//
//	output_base_path = "#{OUTPUT_PATH}/#{CONFIG}"
//	
//	zip_path = "#{output_base_path}/zip"
//	mkdir_p zip_path
//
//	sh "\"#{ZIP_EXE}\" a -r \"#{zip_path}/NSubstitute-#{@@build_number}.zip\" \"#{output_base_path}/NSubstitute-#{@@build_number}\""
)

// TODO
Target "Documentation" DoNothing

Target "Release" DoNothing

"Clean"
   ==> "Version"
   ==> "BuildSolution"
   ==> "Test"
   ==> "Default"
   ==> "NuGet"
   ==> "Documentation"
   ==> "Release"

RunTargetOrDefault "Default"