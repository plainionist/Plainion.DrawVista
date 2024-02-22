
cd %~d0%~p0..

cd src\Plainion.DrawVista\WebUI
pnpm install
pnpm run build

cd ..\..\..
dotnet build --configuration=Release
