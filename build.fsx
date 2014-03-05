#r @"ThirdParty\FAKE\FAKE.Core\tools\FakeLib.dll"
#load @"Build\ExtractDocs.fsx"
open Fake
open Fake.AssemblyInfoFile
open System
open System.IO
open ExtractDocs

module FileReaderWriter =
    let Read file =
        File.ReadAllText(file)

    let Write file text =
        File.WriteAllText(file, text)

module ExamplesToCode =
    let inline curry f a b = f(a,b)
    let TransformFile file target (f : string -> string) =
        File.ReadAllText file
        |> f
        |> curry File.WriteAllText target

    let ConvertFile file targetDir =
         let fileName = Path.GetFileNameWithoutExtension(file)
         let target = (String.Format("{0}{1}.cs", targetDir, fileName))
         log (String.Format("Converting {0} to {1}", file, target))
         TransformFile file target (ExtractDocs.strToFixture fileName)

    let Convert paths targetDir =
        let paths = paths |> Seq.toList

        for p in paths do
            trace (String.Format("Convert from {0} to {1}", p, targetDir))

            let mdFiles = !! (p + "*.markdown") |> Seq.toList
            let htmlFiles = !! (p + "*.html") |> Seq.toList

            for file in List.append mdFiles htmlFiles do
                ConvertFile file targetDir

let EXPERIMENTAL_TARGETS = []

let buildMode = getBuildParamOrDefault "buildMode" "Debug"

let OUTPUT_PATH = "./Output"

let releaseNotes =
    ReadFile "CHANGELOG.txt" 
    |> ReleaseNotesHelper.parseReleaseNotes

let version = releaseNotes.AssemblyVersion

Target "Clean" (fun _ ->
    CleanDirs [ OUTPUT_PATH ]
)

Target "Version" (fun _ ->
    CreateCSharpAssemblyInfo "Source/NSubstitute/Properties/AssemblyInfo.cs"
        [Attribute.Title "NSubstitute"
         Attribute.Description "A simple substitute for .NET mocking frameworks."
         Attribute.Guid "f1571463-8354-493c-b67c-cd9cec9adf78"
         Attribute.Product "NSubstitute"
         Attribute.Copyright @"Copyright \u00A9 2009 NSubstitute Team"
         Attribute.Version version
         Attribute.FileVersion version]
)

Target "BuildSolution" (fun _ ->
  let seqBuild = Seq.map (fun config ->
      MSBuild null "Build" ["Configuration", config] ["./Source/NSubstitute.2010.sln"]
          |> Log "Build: " )

  // TODO: this is a kludge but I don't really care about the result here
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
                OutputFile = outputDir + "TestResults.xml"}) // TODO: different file name based on test assembly
)

Target "Default" DoNothing

let outputBasePath =  String.Format("{0}/{1}/", OUTPUT_PATH, buildMode);
let workingDir = String.Format("{0}package/", outputBasePath)

let net35binary = String.Format("{0}NET35/NSubstitute/NSubstitute.dll", outputDir)
let net40binary = String.Format("{0}NET40/NSubstitute/NSubstitute.dll", outputDir)
let net35binariesDir = String.Format("{0}lib/net35", workingDir)
let net40binariesDir = String.Format("{0}lib/net40", workingDir)

Target "NuGet" (fun _ ->
    CreateDir net35binariesDir
    CreateDir net40binariesDir

    CopyFile net35binariesDir net35binary
    CopyFile net40binariesDir net40binary

    NuGet (fun p ->
        {p with
            OutputPath = outputBasePath
            WorkingDir = workingDir
            Version = version
            ReleaseNotes = toLines releaseNotes.Notes
             }) "Build/NSubstitute.nuspec"
)

// TODO: this is crazy hideous
Target "Zip" (fun _ ->

    // porting markdown files as-is
    CopyFile (workingDir+"acknowledgements.txt") "acknowledgements.markdown"
    CopyFile workingDir "BreakingChanges.txt"
    CopyFile workingDir "ChangeLog.txt"
    CopyFile workingDir "LICENSE.txt"
    CopyFile (workingDir+"README.txt") "README.markdown"

    CreateDir net35binariesDir
    CreateDir net40binariesDir

    CopyFile net35binariesDir net35binary
    CopyFile net40binariesDir net40binary

    let outputZip =  (outputBasePath + "NSubstitute." + version + ".zip")

    !! (workingDir + "/**/*.*")
        -- "*.zip"
        |>Zip workingDir outputZip
)

Target "Documentation" (fun _ -> 
    log "building site..."
    let result = ExecProcess (fun info ->
           info.FileName <- "../../ThirdParty/pretzel/pretzel.exe"
           info.WorkingDirectory <- "./Source/Docs/"
           info.Arguments <- "bake -d ./")

    log ("returned" + result.ToString())
)

Target "CodeFromDocumentation" (fun _ -> 
    let outputCodePath = String.Format("{0}/{1}/", OUTPUT_PATH, "CodeFromDocs")

    CreateDir outputCodePath

    // generate samples from docs
    ExamplesToCode.Convert [ "./Source/Docs/"; "./Source/Docs/help/_posts/"; "./" ] outputCodePath

    // compile code samples
    let basePath = (OUTPUT_PATH + "/" + buildMode + "/NET35")
    let references = [ 
        for x in [ "NSubstitute.dll"; "nunit.framework.dll"] 
            -> String.Format("{0}/NSubstitute.Specs/{1}", basePath, x)]
    CopyFiles outputCodePath references
    CopyFile outputCodePath "./Build/samples.csproj"

    MSBuild null "Build" ["TargetFrameworkVersion", "v3.5"] [ outputCodePath + "/samples.csproj"]
          |> Log "Build: "
)

// empty target to encompass doing everything
Target "Release" DoNothing

"Clean"
   ==> "Version"
   ==> "BuildSolution"
   ==> "Test"
   ==> "Default"
   ==> "NuGet"
   ==> "Zip"
   ==> "Documentation"
   ==> "CodeFromDocumentation"
   ==> "Release"

RunTargetOrDefault "Default"