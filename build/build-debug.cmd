
cd %~d0%~p0..

cd src\Plainion.DrawVista
call pnpm install

cd WebUI

call pnpm install
call pnpm build

cd ..\..\..
dotnet build
