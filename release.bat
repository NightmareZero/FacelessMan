@echo off
echo build with release configuration
call .vscode/build.bat


echo delete old mod file
DEL /Q /S ..\FacelessMan-Release

echo create new mod directory
MD ..\FacelessMan-Release

REM copy new mod files and directories
xcopy /E /I /Y .\1.5 ..\FacelessMan-Release\1.5
xcopy /E /I /Y .\About ..\FacelessMan-Release\About
@REM xcopy /E /I /Y .\Languages ..\FacelessMan-Release\Languages
@REM xcopy /E /I /Y .\Sounds ..\FacelessMan-Release\Sounds
xcopy /E /I /Y .\Textures ..\FacelessMan-Release\Textures