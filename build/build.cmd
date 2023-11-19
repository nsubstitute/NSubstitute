@echo off
cls
set encoding=utf-8

SET SCRIPT_DIR=%~dp0
dotnet run --project "%SCRIPT_DIR%/build.fsproj" -- %*
