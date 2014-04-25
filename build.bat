@echo off

"ThirdParty\NuGet\nuget.exe" "install" "FAKE.Core" "-OutputDirectory" "ThirdParty\FAKE" "-ExcludeVersion" "-version" "2.6.0"

"ThirdParty\FAKE\FAKE.Core\tools\Fake.exe" "build.fsx" %*

rem Bail if we're running a TeamCity build.
if defined TEAMCITY_PROJECT_NAME goto Quit

rem Bail if we're running a MyGet build.
if /i "%BuildRunner%"=="MyGet" goto Quit

:Quit
exit /b %errorlevel%
