
cd %~d0%~p0..

cd src\Plainion.DrawVista\WebUI
npm run build

cd ..\..\..
dotnet build --configuration=Release
