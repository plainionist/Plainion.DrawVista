
cd %~d0%~p0..

cd src\Plainion.DrawVista
call pnpm install

:: use for VueJS with vanilla JS (legacy)
cd WebUI
:: use for VueJS + Quasar + TS
:: cd WebUI_quasar

call pnpm install
call pnpm build

cd ..\..\..
dotnet build --configuration=Release
