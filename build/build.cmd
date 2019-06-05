@echo off
cls
set encoding=utf-8

SET SCRIPT_DIR=%~dp0
SET TOOL_PATH=%SCRIPT_DIR%\.fake

IF NOT EXIST "%TOOL_PATH%\fake.exe" (
  dotnet tool install fake-cli --tool-path %TOOL_PATH%
)

"%TOOL_PATH%/fake.exe" %*