@echo off
echo Press any key to publish
pause
".nuget\NuGet.exe" push Yort.DeadManSwitch.1.0.2.nupkg -Source https://www.nuget.org/api/v2/package
pause