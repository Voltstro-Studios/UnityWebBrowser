if($IsLinux)
{
    dotnet publish ../UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c ReleaseUnity -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=true -p:PublishReadyToRun=true --self-contained true --nologo
}
else
{
    dotnet publish ../UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c ReleaseUnity -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=true --self-contained true --nologo
}

Copy-Item -Path "../UnityWebBrowser.Engine.Cef/bin/ReleaseUnity/linux-x64/publish/*" -Destination "../Packages/UnityWebBrowser.Engine.Cef.Linux-x64/Engine~/" -Recurse -Force -PassThru