if($IsLinux)
{
    dotnet publish ../src/UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c Release -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true -p:PublishReadyToRun=true --self-contained true --nologo
}
else
{
    dotnet publish ../src/UnityWebBrowser.Engine.Cef/UnityWebBrowser.Engine.Cef.csproj -c Release -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=true -p:IncludeAllContentForSelfExtract=true --self-contained true --nologo
}