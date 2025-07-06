@echo off
echo build with release configuration
call .vscode/build.bat
if errorlevel 1 (
    echo WARNING: Build failed. Stopping script execution.
    exit /b
)


echo delete old mod file
DEL /Q /S ..\FacelessMan-Release

echo create new mod directory
MD ..\FacelessMan-Release

REM copy new mod files and directories
xcopy /E /I /Y .\1.5 ..\FacelessMan-Release\1.5
DEL /Q /S ..\FacelessMan-Release\1.5\Assemblies\*.pdb
rmdir /S /Q ..\FacelessMan-Release\1.5\obj
rmdir /S /Q ..\FacelessMan-Release\1.5\Source

xcopy /E /I /Y .\1.6 ..\FacelessMan-Release\1.6
DEL /Q /S ..\FacelessMan-Release\1.6\Assemblies\*.pdb
rmdir /S /Q ..\FacelessMan-Release\1.6\obj
rmdir /S /Q ..\FacelessMan-Release\1.6\Source


xcopy /E /I /Y .\About ..\FacelessMan-Release\About
xcopy /E /I /Y .\Languages ..\FacelessMan-Release\Languages
@REM xcopy /E /I /Y .\Sounds ..\FacelessMan-Release\Sounds
xcopy /E /I /Y .\Textures ..\FacelessMan-Release\Textures
xcopy /Y .\loadFolders.xml ..\FacelessMan-Release\