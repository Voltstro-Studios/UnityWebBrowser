if($IsWindows)
{
    dotnet publish ../UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c ReleaseUnity -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true -p:PublishReadyToRun=true --self-contained true --nologo
}
else
{
    dotnet publish ../UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c ReleaseUnity -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true --self-contained true --nologo
}

Copy-Item -Path "../UnityWebBrowser.Engine.Cef/bin/ReleaseUnity/win-x64/publish/*" -Destination "../Packages/UnityWebBrowser/Plugins/CefBrowser/win-x64/" -Recurse -Force -PassThru