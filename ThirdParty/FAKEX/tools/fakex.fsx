open Fake
open Fake.AppVeyor
open System
open System.IO

// functions 

let inline FileName fullName = Path.GetFileName fullName 

let dnu args =
    ExecProcessWithLambdas (fun info ->
        info.FileName <- (environVar "DNX_FOLDER") + "\dnu.cmd"
        info.Arguments <- args + " --quiet"
    ) TimeSpan.MaxValue true failwith traceImportant
        |> ignore
        
let dnx workingDirectory args =
    ExecProcessWithLambdas (fun info ->
        info.WorkingDirectory <- workingDirectory
        info.FileName <- (environVar "DNX_FOLDER") + "\dnx.exe"
        info.Arguments <- args
    ) TimeSpan.MaxValue true failwith traceImportant
        |> ignore
    
let UpdateVersion version project =
    log ("Updating version in " + project)
    ReplaceInFile (fun s -> s.Replace("1.0.0-ci", version)) project
    
let BuildProject project =
    dnu ("pack --configuration Release " + (DirectoryName project))
    
let CopyArtifact artifact =
    log ("Copying artifact " + (FileName artifact))
    ensureDirectory "artifacts"
    CopyFile "artifacts" artifact

let IsTestProject project = 
    let content = ReadFileAsString project
    content.Contains("\"test\"")
    
let IsXunitProject project = 
    let content = ReadFileAsString project
    content.Contains("\"xunit.runner.dnx\"")
        
let RunTests project =
    if IsTestProject project then    
        let projectDirectory = (DirectoryName project);
    
        if (buildServer = BuildServer.AppVeyor && IsXunitProject project) 
        then
            let tempDirectory = (projectDirectory + "/temp-xunit");
            ensureDirectory tempDirectory
            dnx projectDirectory (". test -xml " + tempDirectory + "/xunit-results.xml")
            UploadTestResultsXml TestResultsType.Xunit tempDirectory
            DeleteDir tempDirectory
        else     
            dnx projectDirectory ". test"

// targets
    
Target "Clean" (fun _ ->
    !! "artifacts" ++ "**/bin"
        |> DeleteDirs
)

Target "UpdateVersions" (fun _ ->
    if buildServer <> BuildServer.LocalBuild then 
        !! "**/project.json"
            |> Seq.iter(UpdateVersion buildVersion)
)

Target "Restore" (fun _ ->
    dnu "restore"
)

Target "BuildProjects" (fun _ ->
    !! "src/**/project.json"
        |> Seq.iter(BuildProject)
)

Target "CopyArtifacts" (fun _ ->    
    !! "src/**/*.nupkg"
        |> Seq.iter(CopyArtifact)
)

Target "RunTests" (fun _ ->
    !! "test/**/project.json"
        |> Seq.iter(RunTests)
)

Target "Build" (fun _ ->)

"Clean"
  ==> "UpdateVersions"
  ==> "Restore"
  ==> "BuildProjects"
  ==> "CopyArtifacts"
  ==> "RunTests"
  ==> "Build"