@echo off

cls
set encoding=utf-8
nuget.exe restore packages.config -PackagesDirectory ..\packages

"..\packages\FAKE.4.60.0\tools\Fake.exe" build.fsx 

rem Bail if we're running a TeamCity build.
if defined TEAMCITY_PROJECT_NAME goto Quit

rem Bail if we're running a MyGet build.
if /i "%BuildRunner%"=="MyGet" goto Quit

:Quit
exit /b %errorlevel%
