# Only Windows can compile Windows Ready to Run
# https://docs.microsoft.com/en-us/dotnet/core/deploying/ready-to-run#cross-platformarchitecture-restrictions

if($IsWindows)
{
    dotnet publish ../UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c ReleaseUnity -r win-x64 -p:PublishReadyToRun=true --self-contained true --nologo
}
else
{
    dotnet publish ../UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c ReleaseUnity -r win-x64 --self-contained true --nologo
}

#Create Engine~/ directory if it doesn't exist
if (-Not (Test-Path "../Packages/UnityWebBrowser.Engine.Cef.Win-x64/Engine~/")) {
    New-Item -Path "../Packages/UnityWebBrowser.Engine.Cef.Win-x64/Engine~/" -ItemType Directory
    New-Item -Path "../Packages/UnityWebBrowser.Engine.Cef.Win-x64/Engine~/swiftshader/" -ItemType Directory
}

Copy-Item -Path "../UnityWebBrowser.Engine.Cef/bin/ReleaseUnity/win-x64/publish/*" -Destination "../Packages/UnityWebBrowser.Engine.Cef.Win-x64/Engine~/" -Recurse -Force -PassThru