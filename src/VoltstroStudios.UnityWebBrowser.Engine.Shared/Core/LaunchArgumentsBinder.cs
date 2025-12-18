// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     <see cref="BinderBase{T}" /> for <see cref="LaunchArguments" />
/// </summary>
internal class LaunchArgumentsBinder : BinderBase<LaunchArguments>
{
    //Initial URL
    private readonly Option<string> initialUrl;
    
    //Initial texture size
    private readonly Option<int> width;
    private readonly Option<int> height;

    //General browser settings
    private readonly Option<int> windowlessFrameRate;
    private readonly Option<bool> javaScript;
    private readonly Option<bool> webRtc;
    private readonly Option<bool> localStorage;
    private readonly Option<int> remoteDebugging;
    private readonly Option<string[]> remoteDebuggingAllowedOrigins;
    private readonly Option<bool> incognitoMode;
    private readonly Option<FileInfo> cachePath;
    private readonly Option<PopupAction> popupAction;
    private readonly Option<bool> noSandbox;
    
    //Background color
    private readonly Option<string> backgroundColor;
    
    //Proxy settings
    private readonly Option<bool> proxyServer;
    private readonly Option<string> proxyUsername;
    private readonly Option<string> proxyPassword;
    
    //Ignore SSL Error Settings
    private readonly Option<bool> ignoreSslErrors;
    private readonly Option<string[]> ignoreSslErrorsDomains;

    //Logging
    private readonly Option<FileInfo> logPath;
    private readonly Option<LogSeverity> logSeverity;
    
    //IPC settings
    private readonly Option<string> communicationLayerName;
    private readonly Option<string> inLocation;
    private readonly Option<string> outLocation;

    //Debugging
    private readonly Option<uint> startDelay;

    public LaunchArgumentsBinder(
        Option<string> initialUrl,
        Option<int> width,
        Option<int> height,
        Option<int> windowlessFrameRate,
        Option<bool> javaScript,
        Option<bool> webRtc,
        Option<bool> localStorage,
        Option<int> remoteDebugging,
        Option<string[]> remoteDebuggingAllowedOrigins,
        Option<bool> incognitoMode,
        Option<FileInfo> cachePath,
        Option<PopupAction> popupAction,
        Option<bool> noSandbox,
        Option<string> backgroundColor,
        Option<bool> proxyServer,
        Option<string> proxyUsername,
        Option<string> proxyPassword,
        Option<bool> ignoreSslErrors,
        Option<string[]> ignoreSslErrorsDomains,
        Option<FileInfo> logPath,
        Option<LogSeverity> logSeverity,
        Option<string> communicationLayerName,
        Option<string> inLocation,
        Option<string> outLocation,
        Option<uint> startDelay)
    {
        this.initialUrl = initialUrl;
        this.width = width;
        this.height = height;

        this.windowlessFrameRate = windowlessFrameRate;
        this.javaScript = javaScript;
        this.webRtc = webRtc;
        this.localStorage = localStorage;
        this.remoteDebugging = remoteDebugging;
        this.remoteDebuggingAllowedOrigins = remoteDebuggingAllowedOrigins;
        this.incognitoMode = incognitoMode;
        this.cachePath = cachePath;
        this.popupAction = popupAction;
        this.noSandbox = noSandbox;
        
        this.backgroundColor = backgroundColor;
        
        this.proxyServer = proxyServer;
        this.proxyUsername = proxyUsername;
        this.proxyPassword = proxyPassword;

        this.ignoreSslErrors = ignoreSslErrors;
        this.ignoreSslErrorsDomains = ignoreSslErrorsDomains;
        
        this.logPath = logPath;
        this.logSeverity = logSeverity;
        
        this.communicationLayerName = communicationLayerName;
        this.inLocation = inLocation;
        this.outLocation = outLocation;

        this.startDelay = startDelay;
    }

    protected override LaunchArguments GetBoundValue(BindingContext bindingContext)
    {
        return new LaunchArguments
        {
            InitialUrl = bindingContext.ParseResult.GetValueForOption(initialUrl),

            Width = bindingContext.ParseResult.GetValueForOption(width),
            Height = bindingContext.ParseResult.GetValueForOption(height),

            WindowlessFrameRate = bindingContext.ParseResult.GetValueForOption(windowlessFrameRate),
            JavaScript = bindingContext.ParseResult.GetValueForOption(javaScript),
            WebRtc = bindingContext.ParseResult.GetValueForOption(webRtc),
            LocalStorage = bindingContext.ParseResult.GetValueForOption(localStorage),
            RemoteDebuggingAllowedOrigins = bindingContext.ParseResult.GetValueForOption(remoteDebuggingAllowedOrigins),
            RemoteDebugging = bindingContext.ParseResult.GetValueForOption(remoteDebugging),
            IncognitoMode = bindingContext.ParseResult.GetValueForOption(incognitoMode),
            CachePath = bindingContext.ParseResult.GetValueForOption(cachePath),
            PopupAction = bindingContext.ParseResult.GetValueForOption(popupAction),
            NoSandbox = bindingContext.ParseResult.GetValueForOption(noSandbox),

            BackgroundColor = new Color(bindingContext.ParseResult.GetValueForOption(backgroundColor)),

            ProxyEnabled = bindingContext.ParseResult.GetValueForOption(proxyServer),
            ProxyUsername = bindingContext.ParseResult.GetValueForOption(proxyUsername),
            ProxyPassword = bindingContext.ParseResult.GetValueForOption(proxyPassword),
            
            IgnoreSslErrors = bindingContext.ParseResult.GetValueForOption(ignoreSslErrors),
            IgnoreSslErrorsDomains = bindingContext.ParseResult.GetValueForOption(ignoreSslErrorsDomains),
            
            LogPath = bindingContext.ParseResult.GetValueForOption(logPath),
            LogSeverity = bindingContext.ParseResult.GetValueForOption(logSeverity),

            CommunicationLayerName = bindingContext.ParseResult.GetValueForOption(communicationLayerName),
            InLocation = bindingContext.ParseResult.GetValueForOption(inLocation),
            OutLocation = bindingContext.ParseResult.GetValueForOption(outLocation),

            StartDelay = bindingContext.ParseResult.GetValueForOption(startDelay)
        };
    }
}