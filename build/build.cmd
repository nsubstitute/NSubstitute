@echo off

cls
set encoding=utf-8

SET SCRIPT_DIR=%~dp0

"%SCRIPT_DIR%\nuget.exe" restore "%SCRIPT_DIR%\packages.config" -PackagesDirectory "%SCRIPT_DIR%\packages"

REM If changing FAKE version, also update build.sh
"%SCRIPT_DIR%\packages/FAKE.4.63.0/tools/Fake.exe" "%SCRIPT_DIR%\build.fsx" %*
