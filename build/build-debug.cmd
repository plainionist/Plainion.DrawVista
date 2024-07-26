
cd %~d0%~p0..

cd src\Plainion.DrawVista\WebUI
call pnpm install

cd ..\..\..
dotnet build
