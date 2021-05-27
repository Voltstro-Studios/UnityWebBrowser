if($IsLinux)
{
    dotnet publish ../src/CefBrowserProcess/CefBrowserProcess.csproj -c Release -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:PublishReadyToRun=true --self-contained true --nologo
}
else
{
    dotnet publish ../src/CefBrowserProcess/CefBrowserProcess.csproj -c Release -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true --self-contained true --nologo
}