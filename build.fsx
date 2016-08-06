#r @"ThirdParty\FAKE\FAKE.Core\tools\FakeLib.dll"
#load @"Build\ExtractDocs.fsx"

open Fake
open Fake.AssemblyInfoFile
open Fake.FileUtils
open System
open System.IO
open ExtractDocs
open System.Text.RegularExpressions

module FileReaderWriter = 
    let Read file = File.ReadAllText(file)
    let Write file text = File.WriteAllText(file, text)
    let TransformFile file target (f : string -> string) = 
        Read file
        |> f
        |> Write target

module ExamplesToCode = 
    open FileReaderWriter
    
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

let NET_TARGETS = [ "NET35"; "NET40"; "NET45" ]
let NETCORE_TARGETS = [ "netstandard1.5" ]
let ALL_TARGETS = List.append NET_TARGETS NETCORE_TARGETS

let buildMode = getBuildParamOrDefault "mode" "Debug"
let targets = 
    match getBuildParamOrDefault "targets" "" with
    | "" -> [ ALL_TARGETS.Head ]
    | "ALL" -> ALL_TARGETS
    | s -> s.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries) |> Array.toList

let packageVersionSuffix = getBuildParamOrDefault "packageVersionSuffix" "" // Use to tag a NuGet build as alpha/beta

let hasAny ofThese = Seq.exists (fun t -> ofThese |> List.contains t)
let hasNetTarget = targets |> hasAny NET_TARGETS
let hasNetCoreTarget = targets |> hasAny NETCORE_TARGETS

let getVersion () =
    let tag = Git.CommandHelper.runSimpleGitCommand "" "describe --tags --long --match v*"
    let result = Regex.Match(tag, @"v(\d+)\.(\d+)\.(\d+)\-(\d+)").Groups
    let getMatch (i:int) = result.[i].Value
    sprintf "%s.%s.%s.%s" (getMatch 1) (getMatch 2) (getMatch 3) (getMatch 4)

let OUTPUT_PATH = "Output"
let releaseNotes = ReadFile "CHANGELOG.txt" |> ReleaseNotesHelper.parseReleaseNotes
let version = getVersion ()
let outputBasePath = OUTPUT_PATH @@ buildMode
let deployPath = outputBasePath @@ "NSubstitute-" + version

Target "Clean" <| fun _ -> CleanDirs [ OUTPUT_PATH ]

Target "Version" <| fun _ ->
    printfn "##teamcity[buildNumber '%s']" version
    CreateCSharpAssemblyInfo "Source/NSubstitute/Properties/AssemblyInfo.cs"
        [ Attribute.Title "NSubstitute"
          Attribute.Description "A simple substitute for .NET mocking libraries." 
          Attribute.Guid "f1571463-8354-493c-b67c-cd9cec9adf78"
          Attribute.Product "NSubstitute" 
          Attribute.Copyright "Copyright \u00A9 2009 NSubstitute Team"
          Attribute.Version version
          Attribute.FileVersion version ]

Target "BuildSolution" <| fun _ -> 
    let build config = 
        MSBuild null "Build"
            [ "Configuration", config + "-" + buildMode ] // e.g. NET35-Debug
            [ "./Source/NSubstitute.2010.sln" ]
            |> ignore
    targets |> List.except NETCORE_TARGETS |> List.iter build

Target "Test" <| fun _ -> 
    targets
    |> List.except NETCORE_TARGETS
    |> List.iter (fun config -> 
                   let workingDir = outputBasePath @@ config
                   let testDlls = !!(workingDir @@ "**" @@ "*Specs.dll")
                   testDlls |> NUnit(fun p -> 
                                   { p with DisableShadowCopy = true
                                            Framework = "net-4.0"
                                            ExcludeCategory = "Pending"
                                            OutputFile = workingDir @@ "TestResults.xml" })
                   )

Target "TestExamples" <| fun _ -> 
    targets
    |> List.except NETCORE_TARGETS
    |> List.iter (fun config -> 
                   let workingDir = outputBasePath
                   let testDlls = !!(workingDir @@ "CodeFromDocs" @@ "NSubstitute.Samples.dll")
                   testDlls |> NUnit(fun p -> 
                                   { p with DisableShadowCopy = true
                                            Framework = "net-4.0"
                                            ExcludeCategory = "Pending"
                                            OutputFile = workingDir @@ "TestResults-codesamples.xml" })
                   )

Target "Default" DoNothing

Target "Package" <| fun _ -> 
    let transform = FileReaderWriter.TransformFile
    let stripHtmlComments (s:string) =
        let commentRegex = Regex.Escape("<!--") + "(.*?)" + Regex.Escape("-->")
        Regex.Replace(s, commentRegex, "", RegexOptions.Singleline)
    targets
    |> List.map (fun x -> x, deployPath @@ "lib" @@ x.ToLower())
    |> List.iter (fun (target, path) -> 
           let nsubDlls = !! "*.dll" ++ "*.xml" |> SetBaseDir (outputBasePath @@ target @@ "NSubstitute")
           CreateDir path
           CopyFiles path nsubDlls
    )

    cp "LICENSE.txt" deployPath
    cp "CHANGELOG.txt" deployPath
    cp "BreakingChanges.txt" deployPath
    cp "README.markdown" (deployPath @@ "README.txt")
    cp "acknowledgements.markdown" (deployPath @@ "acknowledgements.txt")
    transform "README.markdown" (deployPath @@ "README.txt") stripHtmlComments
    transform "acknowledgements.markdown" (deployPath @@ "acknowledgements.txt") stripHtmlComments

Target "NuGet" <| fun _ -> 
    let nugetPath = outputBasePath @@ "nuget"
    let workingDir = nugetPath @@ version
    let versionSuffix = if packageVersionSuffix <> "" then "-" + packageVersionSuffix else ""
    CreateDir workingDir
    cp_r deployPath workingDir

    NuGet (fun p -> 
        { p with OutputPath = nugetPath
                 WorkingDir = workingDir
                 Version = version + versionSuffix
                 ReleaseNotes = toLines releaseNotes.Notes 
                 // TODO better way to get .NET Core dependencies from project.json
                 DependenciesByFramework = 
                     [{ FrameworkVersion = "netstandard1.5"
                        Dependencies = 
                            ["Castle.Core", "[4.0.0-beta001, )"
                             "Microsoft.CSharp", "[4.0.1, )"
                             "NETStandard.Library", "[1.6.0, )"
                             "System.Linq.Queryable", "[4.0.1, )"
                             "System.Reflection.TypeExtensions", "[4.1.0, )" ]}]})
                        "Build/NSubstitute.nuspec"

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

// .NET Core build
#r @"ThirdParty\FAKE.Dotnet\FAKE.Dotnet\tools\Fake.Dotnet.dll"
open Fake
open Fake.Dotnet

Target "InstallDotnetCore" (fun _ ->
    DotnetCliInstall Preview2ToolingOptions
)

Target "BuildDotNetCore" (fun _ ->
    !! "Source/NSubstitute/project.json" 
        |> Seq.iter(fun proj ->  

            // restore project dependencies
            DotnetRestore id proj

            // build project and produce outputs
            DotnetCompile (fun c -> 
                { c with 
                    Configuration = BuildConfiguration.Custom buildMode;
                    Framework = Some ("netstandard1.5");
                    OutputPath = Some (outputBasePath @@ "netstandard1.5" @@ "NSubstitute")
                }) proj
        )
)


// Empty target to encompass doing everything
Target "All" DoNothing

// List targets, similar to `rake -T`
Target "-T" <| fun _ ->
    printfn "Optional config options:"
    printfn "  mode=Debug|Release"
    printfn "  targets=ALL|%s" (String.concat "|" ALL_TARGETS)
    printfn "  packageVersionSuffix=alpha|beta|beta2|...   - used to tag a NuGet package as prerelease"
    printfn ""
    PrintTargets()

// Build. Will build a mix of dotnet core and/or NETxx depending on targets
Target "Build" DoNothing

"InstallDotnetCore" ==> "BuildDotNetCore"
"BuildDotNetCore" =?> ("Build", hasNetCoreTarget)
"BuildSolution" =?> ("Build", hasNetTarget)

// If Clean or Version runs, make sure it runs before either build step
"Clean" ?=> "BuildDotNetCore"
"Clean" ?=> "BuildSolution"
"Version" ?=> "BuildDotNetCore"
"Version" ?=> "BuildSolution"

"Clean"
    ==> "Version"
    ==> "Build"
    ==> "Test"
    ==> "Default"

// Full build
"Default"
    ==> "CodeFromDocumentation"
    ==> "TestExamples"
    ==> "Package"
    ==> "NuGet"
    ==> "Zip" 
    ==> "Documentation"
    ==> "All"


RunTargetOrDefault "Default"
