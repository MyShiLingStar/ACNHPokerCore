@echo off
FOR /F "delims=" %%i IN ('dotnet msbuild ACNHPokerCore/ACNHPokerCore.csproj --getProperty:TargetFramework') DO (SET netversion=%%i)
echo netversion is: %netversion%
echo %netversion%>>netversion.txt