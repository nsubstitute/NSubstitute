#r @"ThirdParty\FAKE\FAKE.Core\tools\FakeLib.dll"
#load @"Build\ExtractDocs.fsx"

open Fake
open Fake.AssemblyInfoFile
open Fake.FileUtils
open System
open System.IO
open ExtractDocs

module FileReaderWriter = 
    let Read file = File.ReadAllText(file)
    let Write file text = File.WriteAllText(file, text)

module ExamplesToCode = 
    open FileReaderWriter
    
    let TransformFile file target (f : string -> string) = 
        Read file
        |> f
        |> Write target
    
    let ConvertFile file targetDir = 
        let fileName = Path.GetFileNameWithoutExtension(file)
        let target = (String.Format("{0}{1}.cs", targetDir, fileName))
        log (String.Format("Converting {0} to {1}", file, target))
        TransformFile file target (ExtractDocs.strToFixture fileName)
    
    let Convert paths targetDir = 
        let paths = paths |> Seq.toList
        for p in paths do
            trace (String.Format("Convert from {0} to {1}", p, targetDir))
            let mdFiles = !!(p + "*.markdown") |> Seq.toList
            let htmlFiles = !!(p + "*.html") |> Seq.toList
            for file in List.append mdFiles htmlFiles do
                ConvertFile file targetDir

let ALL_TARGETS = [ "NET35"; "NET40"; "NET45" ]
let buildMode = getBuildParamOrDefault "mode" "Debug"

let targets = 
    match getBuildParamOrDefault "targets" "" with
    | "" -> [ ALL_TARGETS.Head ]
    | "ALL" -> ALL_TARGETS
    | s -> s.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries) |> Array.toList

let OUTPUT_PATH = "Output"
let outputDir = OUTPUT_PATH @@ buildMode
let releaseNotes = ReadFile "CHANGELOG.txt" |> ReleaseNotesHelper.parseReleaseNotes
let version = releaseNotes.AssemblyVersion

Target "Clean" (fun _ -> CleanDirs [ OUTPUT_PATH ])
Target "Version" (fun _ -> 
    CreateCSharpAssemblyInfo "Source/NSubstitute/Properties/AssemblyInfo.cs"
        [ Attribute.Title "NSubstitute"
          Attribute.Description "A simple substitute for .NET mocking libraries." 
          Attribute.Guid "f1571463-8354-493c-b67c-cd9cec9adf78"
          Attribute.Product "NSubstitute" 
          Attribute.Copyright @"Copyright \u00A9 2009 NSubstitute Team"
          Attribute.Version version
          Attribute.FileVersion version ])

Target "BuildSolution" (fun _ -> 
    let build config mode = 
        MSBuild null "Build"
            [ "Configuration", config + "-" + buildMode ] // e.g. NET35-Debug
            [ "./Source/NSubstitute.2010.sln" ]
            |> ignore
    targets |> List.iter (fun config -> build config buildMode))

Target "Test" (fun _ -> 
    targets
    |> List.iter (fun config -> 
                   let workingDir = outputDir @@ config
                   let testDlls = !!(workingDir @@ "**/*Specs.dll")
                   testDlls |> NUnit(fun p -> 
                                   { p with DisableShadowCopy = true
                                            Framework = "net-4.0"
                                            ExcludeCategory = "Pending"
                                            OutputFile = workingDir @@ "TestResults.xml" }) // TODO: different file name based on test assembly
       )
)

Target "Default" DoNothing

let outputBasePath = OUTPUT_PATH @@ buildMode
let workingDir = outputBasePath @@ "package"
let net35binary = String.Format("{0}NET35/NSubstitute/NSubstitute.dll", outputDir)
let net40binary = String.Format("{0}NET40/NSubstitute/NSubstitute.dll", outputDir)
let net35binariesDir = String.Format("{0}lib/net35", workingDir)
let net40binariesDir = String.Format("{0}lib/net40", workingDir)

Target "Package" (fun _ -> 
    let deployPath = outputBasePath @@ "NSubstitute-" + version
    targets
    |> List.map (fun x -> x, deployPath @@ "lib" @@ x.ToLower())
    |> List.iter (fun (target, path) -> 
           let nsubDlls = !! "*.dll" ++ "*.xml" |> SetBaseDir (outputBasePath @@ target @@ "NSubstitute")
           CreateDir path
           CopyFiles path nsubDlls
    )
    cp "README.markdown" (deployPath @@ "README.txt")
    cp "LICENSE.txt" deployPath
    cp "CHANGELOG.txt" deployPath
    cp "BreakingChanges.txt" deployPath
    cp "acknowledgements.markdown" (deployPath @@ "acknowledgements.txt")
(*
    tidyUpTextFileFromMarkdown("#{deploy_path}/README.txt")
    tidyUpTextFileFromMarkdown("#{deploy_path}/acknowledgements.txt")
    *)
)

Target "NuGet" (fun _ -> 
    let workingDir = outputBasePath @@ "package"
    targets
    |> List.map (fun x -> workingDir @@ "lib" @@ x.ToLower())
    |> List.iter CreateDir
    let z = CopyFiles
    CopyFile net35binariesDir net35binary
    CopyFile net40binariesDir net40binary
    NuGet (fun p -> 
        { p with OutputPath = outputBasePath
                 WorkingDir = workingDir
                 Version = version
                 ReleaseNotes = toLines releaseNotes.Notes }) "Build/NSubstitute.nuspec")

// TODO: this is crazy hideous
Target "Zip" (fun _ -> 
    // porting markdown files as-is
    CopyFile (workingDir + "acknowledgements.txt") "acknowledgements.markdown"
    CopyFile workingDir "BreakingChanges.txt"
    CopyFile workingDir "ChangeLog.txt"
    CopyFile workingDir "LICENSE.txt"
    CopyFile (workingDir + "README.txt") "README.markdown"
    CreateDir net35binariesDir
    CreateDir net40binariesDir
    CopyFile net35binariesDir net35binary
    CopyFile net40binariesDir net40binary
    let outputZip = (outputBasePath + "NSubstitute." + version + ".zip")
    !!(workingDir + "/**/*.*") -- "*.zip"
    |> Zip workingDir outputZip)

Target "Documentation" (fun _ -> 
    log "building site..."
    let result = 
        ExecProcess(fun info -> 
            info.FileName <- "bundle exec jekyll"
            info.WorkingDirectory <- "./Source/Docs/"
            info.Arguments <- "\"" + outputBasePath @@ "nsubstitute.github.com" + "\"")
    log ("returned" + result.ToString()))

Target "CodeFromDocumentation" (fun _ -> 
    let outputCodePath = String.Format("{0}/{1}/", OUTPUT_PATH, "CodeFromDocs")
    CreateDir outputCodePath
    // generate samples from docs
    ExamplesToCode.Convert [ "./Source/Docs/"; "./Source/Docs/help/_posts/"; "./" ] outputCodePath
    // compile code samples
    let basePath = (OUTPUT_PATH + "/" + buildMode + "/NET35")
    
    let references = 
        [ for x in [ "NSubstitute.dll"; "nunit.framework.dll" ] -> 
              String.Format("{0}/NSubstitute.Specs/{1}", basePath, x) ]
    CopyFiles outputCodePath references
    CopyFile outputCodePath "./Build/samples.csproj"
    MSBuild null "Build" [ "TargetFrameworkVersion", "v3.5" ] [ outputCodePath + "/samples.csproj" ] |> Log "Build: ")

// empty target to encompass doing everything
Target "Release" DoNothing

"Clean"
    ==> "Version"
    ==> "BuildSolution"
    ==> "Test"
    ==> "Default"
    ==> "Package"
    ==> "NuGet"
    ==> "Zip" 
    ==> "Documentation"
    ==> "CodeFromDocumentation"
    ==> "Release"
RunTargetOrDefault "Default"
