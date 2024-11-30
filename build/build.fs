open System
open System.Diagnostics
open System.IO

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators

open ExtractDocs

let target = Target.create
let description = Target.description

module FileReaderWriter =
    let Read file = File.ReadAllText(file)
    let Write file (text: string) = File.WriteAllText(file, text)
    let TransformFile file target (f : string -> string) =
        Read file
        |> f
        |> Write target

module ExamplesToCode =
    open FileReaderWriter

    let ConvertFile (file: string) targetDir =
        let fileName = Path.GetFileNameWithoutExtension(file)
        let target = targetDir @@ fileName + ".cs"
        Trace.log <| sprintf "Converting %s to %s" file target
        TransformFile file target (ExtractDocs.strToFixture fileName)

    let Convert paths targetDir =
        let paths = paths |> Seq.toList
        for p in paths do
            Trace.trace <| sprintf "Convert from %s to %s" p targetDir
            let files = !! "*.markdown" ++ "*.html" ++ "*.md" |> GlobbingPattern.setBaseDir p
            for file in files do
                ConvertFile file targetDir

type BuildVersion = { assembly: string; file: string; info: string; package: string }

let root = __SOURCE_DIRECTORY__ </> ".." |> Path.getFullName

let configuration = Environment.environVarOrDefault "configuration" "Debug"

let output = root </> "bin" </> configuration
let solution = (root </> "NSubstitute.sln")

let initTargets() =
    Target.description("Extract, build and test code from documentation.")
    Target.create "TestCodeFromDocs" <| fun _ ->
        let outputCodePath = output </> "CodeFromDocs"
        Directory.create outputCodePath
        // generate samples from docs
        ExamplesToCode.Convert [ root </> "docs/"; root </> "docs/help/_posts/"; root ] outputCodePath
        // compile code samples
        let csproj = """
            <Project Sdk="Microsoft.NET.Sdk">
            <PropertyGroup>
                <TargetFrameworks>net8.0;net462</TargetFrameworks>
                <LangVersion>latest</LangVersion>
            </PropertyGroup>
            <ItemGroup>
                <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
                <PackageReference Include="NUnit" Version="3.14.0" />
                <PackageReference Include="NUnit3TestAdapter" Version="4.6.0" />
            </ItemGroup>
            <ItemGroup>
                <ProjectReference Include="..\..\..\src\NSubstitute\NSubstitute.csproj" />
            </ItemGroup>
        </Project>
        """
        let projPath = outputCodePath </> "Docs.csproj"
        FileReaderWriter.Write projPath csproj
        DotNet.restore (fun p -> p) projPath
        DotNet.build (fun p -> p) projPath
        DotNet.test (fun p -> p) projPath

    let tryFindFileOnPath (file : string) : string option =
        Environment.GetEnvironmentVariable("PATH").Split([| Path.PathSeparator |])
        |> Seq.append ["."]
        |> fun path -> ProcessUtils.tryFindFile path file

    Target.description("Build documentation website. Requires Ruby, bundler and jekyll.")
    Target.create "Documentation" <| fun _ -> 
        Trace.log "Building site..."
        let exe = [ "bundle.bat"; "bundle" ]
                    |> Seq.map tryFindFileOnPath
                    |> Seq.collect (Option.toList)
                    |> Seq.tryFind (fun _ -> true)
                    |> function | Some x -> Trace.log ("using " + x); x
                                | None   -> Trace.log ("count not find exe"); "bundle"

        let workingDir = root </> "docs/"
        let docOutputRelativeToWorkingDir = ".." </> output </> "nsubstitute.github.com"
        
        // TODO migrate the following to FAKE API: CreateProcess.ofStartInfo(p)
        // https://fake.build/apidocs/v5/fake-core-createprocess.html
        // that doesn't work for some reason
        let p = ProcessStartInfo(
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        FileName = exe,
                        WorkingDirectory = workingDir,
                        Arguments = "exec jekyll build -d \"" + docOutputRelativeToWorkingDir + "\"")
        let proc = Process.Start(p)
        proc.WaitForExit()
        let result = proc.ExitCode
        if result = 0 then
            "Site built in " + docOutputRelativeToWorkingDir |> Trace.log
        else
            "failed to build site" |> failwith

    Target.description("List targets, similar to `rake -T`. For more details, run `--listTargets` instead.")
    Target.create "-T" <| fun _ ->
        printfn "Optional config options:"
        printfn "  configuration=Debug|Release"
        printfn "  benchmark=*|<benchmark name>  (only for Benchmarks target in Release mode)"
        printfn ""
        Target.listAvailable()

    "Documentation" <== [ "TestCodeFromDocs" ]

[<EntryPoint>]
let main argv =
    argv
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext
    initTargets()
    Target.runOrDefaultWithArguments "TestCodeFromDocs"
    0 
