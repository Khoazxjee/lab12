@echo off
cd /d "%~dp0"
dotnet run --project "Lab5\Lab5.csproj" --launch-profile Lab5
if errorlevel 1 pause
