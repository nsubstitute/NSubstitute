@echo off

rem todo do not install dnvm all the time
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}"

rem todo remove hardcoded version
CALL dnvm install '1.0.0-rc2-16319' -r coreclr -arch x64 -a default

CALL dnvm use default -runtime coreclr -arch x64
SET DNX_FOLDER=%USERPROFILE%\.dnx\runtimes\dnx-coreclr-win-x64.1.0.0-rc2-16319\bin

"ThirdParty\FAKE\FAKE.Core\tools\Fake.exe" "build.fsx" %*

rem Bail if we're running a TeamCity build.
if defined TEAMCITY_PROJECT_NAME goto Quit

rem Bail if we're running a MyGet build.
if /i "%BuildRunner%"=="MyGet" goto Quit

:Quit
exit /b %errorlevel%
