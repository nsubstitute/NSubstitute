@echo off

cls
set encoding=utf-8

SET TARGET="Default"
IF NOT [%1]==[] (set TARGET="%1")

SET CONFIGURATION="Debug"
IF NOT [%2]==[] (set CONFIGURATION="%2")

pushd %~dp0..\
"build/nuget.exe" restore "build/packages.config " -PackagesDirectory "packages"
"packages/FAKE.4.63.0/tools/Fake.exe" "build/build.fsx" "target=%TARGET%" "configuration=%CONFIGURATION%"
popd
