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
        let target = targetDir @@ fileName + ".cs"
        log <| sprintf "Converting %s to %s" file target
        TransformFile file target (ExtractDocs.strToFixture fileName)
    
    let Convert paths targetDir = 
        let paths = paths |> Seq.toList
        for p in paths do
            trace <| sprintf "Convert from %s to %s" p targetDir
            let files = !! "*.markdown" ++ "*.html" |> SetBaseDir p
            for file in files do
                ConvertFile file targetDir

let ALL_TARGETS = [ "NET35"; "NET40"; "NET45" ]
let buildMode = getBuildParamOrDefault "mode" "Debug"

let targets = 
    match getBuildParamOrDefault "targets" "" with
    | "" -> [ ALL_TARGETS.Head ]
    | "ALL" -> ALL_TARGETS
    | s -> s.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries) |> Array.toList

let OUTPUT_PATH = "Output"
let releaseNotes = ReadFile "CHANGELOG.txt" |> ReleaseNotesHelper.parseReleaseNotes
let version = releaseNotes.AssemblyVersion
let outputBasePath = OUTPUT_PATH @@ buildMode
let deployPath = outputBasePath @@ "NSubstitute-" + version

Target "Clean" <| fun _ -> CleanDirs [ OUTPUT_PATH ]

Target "Version" <| fun _ ->
    CreateCSharpAssemblyInfo "Source/NSubstitute/Properties/AssemblyInfo.cs"
        [ Attribute.Title "NSubstitute"
          Attribute.Description "A simple substitute for .NET mocking libraries." 
          Attribute.Guid "f1571463-8354-493c-b67c-cd9cec9adf78"
          Attribute.Product "NSubstitute" 
          Attribute.Copyright @"Copyright \u00A9 2009 NSubstitute Team"
          Attribute.Version version
          Attribute.FileVersion version ]

Target "BuildSolution" <| fun _ -> 
    let build config mode = 
        MSBuild null "Build"
            [ "Configuration", config + "-" + buildMode ] // e.g. NET35-Debug
            [ "./Source/NSubstitute.2010.sln" ]
            |> ignore
    targets |> List.iter (fun config -> build config buildMode)

Target "Test" <| fun _ -> 
    targets
    |> List.iter (fun config -> 
                   let workingDir = outputBasePath @@ config
                   let testDlls = !!(workingDir @@ "**" @@ "*Specs.dll")
                   testDlls |> NUnit(fun p -> 
                                   { p with DisableShadowCopy = true
                                            Framework = "net-4.0"
                                            ExcludeCategory = "Pending"
                                            OutputFile = workingDir @@ "TestResults.xml" })
                   )

Target "Default" DoNothing

Target "Package" <| fun _ -> 
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

Target "NuGet" <| fun _ -> 
    let nugetPath = outputBasePath @@ "nuget"
    let workingDir = nugetPath @@ version
    CreateDir workingDir
    cp_r deployPath workingDir

    NuGet (fun p -> 
        { p with OutputPath = nugetPath
                 WorkingDir = workingDir
                 Version = version
                 ReleaseNotes = toLines releaseNotes.Notes }) "Build/NSubstitute.nuspec"

Target "Zip" <| fun _ -> 
    let zipPath = outputBasePath @@ "zip"
    CreateDir zipPath
    let outputZip = (zipPath @@ "NSubstitute." + version + ".zip")
    !!(deployPath @@ "**" @@ "*.*") -- "*.zip"
    |> Zip deployPath outputZip

let tryFindFileOnPath (file : string) : string option =
    Environment.GetEnvironmentVariable("PATH").Split([| Path.PathSeparator |])
    |> Seq.append ["."]
    |> fun path -> tryFindFile path file

Target "Documentation" <| fun _ -> 
    log "building site..."
    let exe = [ "bundle.bat"; "bundle" ]
                |> Seq.map tryFindFileOnPath
                |> Seq.collect (Option.toList)
                |> Seq.tryFind (fun _ -> true)
                |> function | Some x -> log ("using " + x); x
                            | None   -> log ("count not find exe"); "bundle"

    let workingDir = "./Source/Docs/"
    let docOutputRelativeToWorkingDir = ".." @@ ".." @@ outputBasePath @@ "nsubstitute.github.com"
    let result = 
        ExecProcess (fun info -> 
                        info.UseShellExecute <- false
                        info.CreateNoWindow <- true
                        info.FileName <- exe
                        info.WorkingDirectory <- workingDir
                        info.Arguments <- "exec jekyll \"" + docOutputRelativeToWorkingDir + "\"")
                    (TimeSpan.FromMinutes 5.)
    if result = 0 then
        "site built in " + docOutputRelativeToWorkingDir |> log
    else
        "failed to build site" |> failwith

Target "CodeFromDocumentation" <| fun _ -> 
    let outputCodePath = outputBasePath @@ "CodeFromDocs"
    CreateDir outputCodePath
    // generate samples from docs
    ExamplesToCode.Convert [ "./Source/Docs/"; "./Source/Docs/help/_posts/"; "./" ] outputCodePath
    // compile code samples
    let basePath = outputBasePath @@ "/NET35"
    
    let references = [ "NSubstitute.dll"; "nunit.framework.dll" ] |> List.map (fun x -> basePath @@ "NSubstitute.Specs" @@ x)
    CopyFiles outputCodePath references
    CopyFile outputCodePath "./Build/samples.csproj"
    MSBuild null "Build" [ "TargetFrameworkVersion", "v3.5" ] [ outputCodePath + "/samples.csproj" ] |> Log "Build: "

// empty target to encompass doing everything
Target "Release" DoNothing

// list targets, similar to `rake -T`
Target "-T" PrintTargets

"Clean"
    ==> "Version"
    ==> "BuildSolution"
    ==> "Test"
    ==> "Default"
    ==> "Package"
    ==> "NuGet"
    ==> "Zip" 
    ==> "CodeFromDocumentation"
    ==> "Documentation"
    ==> "Release"

RunTargetOrDefault "Default"
