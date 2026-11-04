@echo off
cd /d "%~dp0"
dotnet run --project "Lab6\Lab6.csproj" --launch-profile Lab6
if errorlevel 1 pause
