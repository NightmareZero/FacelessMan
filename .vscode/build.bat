@echo off

REM Check if a build configuration (Debug/Release) is passed as an argument
if "%1"=="" (
    set BUILD_CONFIG=Release
    REM remove unnecessary assemblies
    DEL /Q .\1.6\Assemblies\*.*
) else (
    set BUILD_CONFIG=%1
)



REM build dll with specified configuration
dotnet build 1.6 -c %BUILD_CONFIG%