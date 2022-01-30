# Any platform can compile Linux Ready to Run
# https://docs.microsoft.com/en-us/dotnet/core/deploying/ready-to-run#cross-platformarchitecture-restrictions

dotnet publish ../UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c ReleaseUnity -r linux-x64 -p:PublishReadyToRun=true --self-contained true --nologo

#Create Engine~/ directory if it doesn't exist
if (-Not (Test-Path "../Packages/UnityWebBrowser.Engine.Cef.Linux-x64/Engine~/")) {
    New-Item -Path "../Packages/UnityWebBrowser.Engine.Cef.Linux-x64/Engine~/" -ItemType Directory
    New-Item -Path "../Packages/UnityWebBrowser.Engine.Cef.Linux-x64/Engine~/swiftshader/" -ItemType Directory
}

Copy-Item -Path "../UnityWebBrowser.Engine.Cef/bin/ReleaseUnity/linux-x64/publish/*" -Destination "../Packages/UnityWebBrowser.Engine.Cef.Linux-x64/Engine~/" -Recurse -Force -PassThru