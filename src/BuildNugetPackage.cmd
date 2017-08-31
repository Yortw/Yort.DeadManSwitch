del /F /Q /S *.CodeAnalysisLog.xml

".nuget\NuGet.exe" pack -sym Yort.DeadManSwitch.nuspec -BasePath .\
pause

copy *.nupkg C:\Nuget.LocalRepository\
pause
