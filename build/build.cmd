@echo off

cls
set encoding=utf-8

pushd %~dp0..\

"build/nuget.exe" restore "build/packages.config " -PackagesDirectory "packages"

REM If changing FAKE version, also update build.sh
"packages/FAKE.4.63.0/tools/Fake.exe" "build/build.fsx" %*
popd
