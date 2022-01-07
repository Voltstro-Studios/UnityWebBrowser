using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Engine.Shared;

public class LaunchArgumentsBinder : BinderBase<LaunchArguments>
{
    private readonly Option<string> initialUrl;

    private readonly Option<int> width;
    private readonly Option<int> height;

    private readonly Option<bool> javaScript;
    private readonly Option<bool> webRtc;
    private readonly Option<int> remoteDebugging;
    private readonly Option<FileInfo> cachePath;

    private readonly Option<Color> backgroundColor;

    private readonly Option<bool> proxyServer;
    private readonly Option<string> proxyUsername;
    private readonly Option<string> proxyPassword;

    private readonly Option<bool> pipes;
    private readonly Option<string> inLocation;
    private readonly Option<string> outLocation;

    private readonly Option<FileInfo> logPath;
    private readonly Option<LogSeverity> logSeverity;

    public LaunchArgumentsBinder(Option<string> initialUrl, 
        Option<int> width, Option<int> height, 
        Option<bool> javaScript, Option<bool> webRtc, Option<int> remoteDebugging, Option<FileInfo> cachePath, 
        Option<Color> backgroundColor,
        Option<bool> proxyServer, Option<string> proxyUsername, Option<string> proxyPassword, 
        Option<bool> pipes, Option<string> inLocation, Option<string> outLocation, 
        Option<FileInfo> logPath, Option<LogSeverity> logSeverity)
    {
        this.initialUrl = initialUrl;
        this.width = width;
        this.height = height;
        this.javaScript = javaScript;
        this.webRtc = webRtc;
        this.remoteDebugging = remoteDebugging;
        this.cachePath = cachePath;
        this.backgroundColor = backgroundColor;
        this.proxyServer = proxyServer;
        this.proxyUsername = proxyUsername;
        this.proxyPassword = proxyPassword;
        this.pipes = pipes;
        this.inLocation = inLocation;
        this.outLocation = outLocation;
        this.logPath = logPath;
        this.logSeverity = logSeverity;
    }

    protected override LaunchArguments GetBoundValue(BindingContext bindingContext)
    {
        return new LaunchArguments
        {
            InitialUrl = bindingContext.ParseResult.GetValueForOption(initialUrl),
            
            Width = bindingContext.ParseResult.GetValueForOption(width),
            Height = bindingContext.ParseResult.GetValueForOption(height),
            
            JavaScript = bindingContext.ParseResult.GetValueForOption(javaScript),
            WebRtc = bindingContext.ParseResult.GetValueForOption(webRtc),
            RemoteDebugging = bindingContext.ParseResult.GetValueForOption(remoteDebugging),
            
            BackgroundColor = bindingContext.ParseResult.GetValueForOption(backgroundColor),

            CachePath = bindingContext.ParseResult.GetValueForOption(cachePath),
            
            ProxyEnabled = bindingContext.ParseResult.GetValueForOption(proxyServer),
            ProxyUsername = bindingContext.ParseResult.GetValueForOption(proxyUsername),
            ProxyPassword = bindingContext.ParseResult.GetValueForOption(proxyPassword),
            
            Pipes = bindingContext.ParseResult.GetValueForOption(pipes),
            InLocation = bindingContext.ParseResult.GetValueForOption(inLocation),
            OutLocation = bindingContext.ParseResult.GetValueForOption(outLocation),
            
            LogPath = bindingContext.ParseResult.GetValueForOption(logPath),
            LogSeverity = bindingContext.ParseResult.GetValueForOption(logSeverity),
        };
    }
}