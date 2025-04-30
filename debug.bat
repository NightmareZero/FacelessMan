@echo off
echo build with release configuration
call .vscode/build.bat debug

if errorlevel 1 (
    echo WARNING: Build failed. Stopping script execution.
    exit /b
)