open System.IO

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.IO.FileSystemOperators

open ExtractDocs

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


let root = __SOURCE_DIRECTORY__ </> ".." |> Path.getFullName

let configuration = Environment.environVarOrDefault "configuration" "Debug"

let output = root </> "bin" </> configuration

[<EntryPoint>]
let main argv =
    let outputCodePath = output </> "CodeFromDocs"
    Directory.create outputCodePath
    // generate samples from docs
    ExamplesToCode.Convert [ root </> "docs/"; root </> "docs/docs"; root ] outputCodePath
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
    0
