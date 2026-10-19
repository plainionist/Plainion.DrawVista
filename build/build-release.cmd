@echo off
setlocal
cd /d "%~dp0.."
if errorlevel 1 exit /b %errorlevel%

cd src\Plainion.DrawVista
call pnpm install --frozen-lockfile
if errorlevel 1 exit /b %errorlevel%

cd WebUI

call pnpm install --frozen-lockfile
if errorlevel 1 exit /b %errorlevel%

cd ..\..\..
dotnet build --configuration=Release
exit /b %errorlevel%
