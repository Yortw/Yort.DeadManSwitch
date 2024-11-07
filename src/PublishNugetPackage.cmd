@echo off
echo Press any key to publish ".\Yort.DeadManSwitch\bin\Release\%1"
pause
"..\.nuget\NuGet.exe" push ".\Yort.DeadManSwitch\bin\Release\%1" -Source https://www.nuget.org/api/v2/package
pause