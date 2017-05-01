@echo off

cls
set encoding=utf-8
pushd %~dp0..\
"build/nuget.exe" restore "build/packages.config " -PackagesDirectory "packages"
"packages/FAKE.4.60.0/tools/Fake.exe" "build/build.fsx"
popd
