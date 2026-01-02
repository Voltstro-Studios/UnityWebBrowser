// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.CommandLine;
using System.IO;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     Parses arguments from the command line
/// </summary>
public sealed class LaunchArgumentsParser
{
    private readonly RootCommand rootCommand;
    
    //Url to start with
    private readonly Option<string> initialUrl = new("-initial-url")
    {
        DefaultValueFactory = _ => "https://voltstro.dev",
        Description = "The initial URL that the browser will first load to."
    };

    //Resolution
    private readonly Option<int> width = new("-width")
    {
        DefaultValueFactory = _ => 1920,
        Description = "The width of the window."
    };
    private readonly Option<int> height = new("-height")
    {
        DefaultValueFactory = _ => 1080,
        Description = "The height of the window."
    };

    //General browser settings
    private readonly Option<int> windowlessFrameRate = new("-windowless-frame-rate")
    {
        DefaultValueFactory = _ => 30,
        Description = "Target framerate for windowless rendering (1-60). Default is 30."
    };
    private readonly Option<bool> javaScript = new("-javascript")
    {
        DefaultValueFactory = _ => true,
        Description = "Enable or disable javascript."
    };
    private readonly Option<bool> webRtc = new("-web-rtc")
    {
        DefaultValueFactory = _ => false,
        Description = "Enable or disable web RTC."
    };
    private readonly Option<bool> localStorage = new("-local-storage")
    {
        DefaultValueFactory = _ => true,
        Description = "Enable or disable local storage."
    };
    private readonly Option<int> remoteDebugging = new("-remote-debugging")
    {
        DefaultValueFactory = _ => 0,
        Description = "If the engine has remote debugging, what port to use (0 for disable)."
    };
    private readonly Option<string[]> remoteDebuggingAllowedOrigins = new("-remote-debugging-allowed-origins")
    {
        DefaultValueFactory = _ => new[] { "http://127.0.0.1:9022" },
        Description = "Allowed origins for remote debugging."
    };
    private readonly Option<bool> incognitoMode = new("-incognito-mode")
    {
        DefaultValueFactory = _ => false,
        Description = "Run the browser in incognito/private mode."
    };
    private readonly Option<FileInfo> cachePath = new("-cache-path")
    {
        DefaultValueFactory = _ => null,
        Description = "The path to the cache (null for no cache)."
    };
    private readonly Option<PopupAction> popupAction = new("-popup-action")
    {
        DefaultValueFactory = _ => PopupAction.Ignore,
        Description = "What action to take when dealing with a popup."
    };
    private readonly Option<bool> noSandbox = new("-no-sandbox")
    {
        DefaultValueFactory = _ => false,
        Description = "Disables browser engine sandboxing."
    };

    //Background color
    private readonly Option<string> backgroundColor = new("-background-color")
    {
        DefaultValueFactory = _ => "ffffffff",
        Description = "The color to use for the background."
    };

    //Proxy settings
    private readonly Option<bool> proxyServer = new("-proxy-server")
    {
        DefaultValueFactory = _ => true,
        Description = "Use a proxy server or direct connect."
    };
    private readonly Option<string> proxyUsername = new("-proxy-username")
    {
        DefaultValueFactory = _ => null,
        Description = "The username to use in the proxy auth."
    };
    private readonly Option<string> proxyPassword = new("-proxy-password")
    {
        DefaultValueFactory = _ => null,
        Description = "The password to use in the proxy auth."
    };
    
    //Ignore SSL Errors
    private readonly Option<bool> ignoreSslErrors = new("-ignore-ssl-errors")
    {
        DefaultValueFactory = _ => false,
        Description = "Will ignore SSL errors on provided domains in ignoreSSLErrorsDomains."
    };
    private readonly Option<string[]> ignoreSslErrorsDomains = new("-ignore-ssl-errors-domains")
    {
        DefaultValueFactory = _ => null,
        Description = "Domains to ignore SSL errors on if ignoreSSLErrors is enabled."
    };
    
    //Logging
    private readonly Option<FileInfo> logPath = new("-log-path")
    {
        DefaultValueFactory = _ => new FileInfo("engine.log"),
        Description = "The path to where the log file will be."
    };
    private readonly Option<LogSeverity> logSeverity = new("-log-severity")
    {
        DefaultValueFactory = _ => LogSeverity.Info,
        Description = "The severity of the logs."
    };

    //IPC settings
    private readonly Option<string> communicationLayerName = new("-comms-layer-name")
    {
        DefaultValueFactory = _ => "TCP",
        Description = "The name of the communication layer to use."
    };
    private readonly Option<string> inLocation = new("-in-location")
    {
        DefaultValueFactory = _ => "5555",
        Description = "In location for IPC (Pipes location or TCP port in TCP mode)."
    };
    private readonly Option<string> outLocation = new("-out-location")
    {
        DefaultValueFactory = _ => "5556",
        Description = "Out location for IPC (Pipes location or TCP port in TCP mode)."
    };

    //Debugging
    private readonly Option<uint> startDelay = new("-start-delay")
    {
        DefaultValueFactory = _ => 0,
        Description = "Delays the starting process. Used for testing reasons."
    };
    
    public LaunchArgumentsParser()
    {
        rootCommand = new RootCommand()
        {
            initialUrl,
            width,
            height,
            windowlessFrameRate,
            javaScript,
            webRtc,
            localStorage,
            remoteDebugging,
            remoteDebuggingAllowedOrigins,
            incognitoMode,
            cachePath,
            popupAction,
            backgroundColor,
            proxyServer,
            proxyUsername,
            proxyPassword,
            ignoreSslErrors,
            ignoreSslErrorsDomains,
            logPath,
            logSeverity,
            communicationLayerName,
            inLocation,
            outLocation,
            startDelay
        };
        rootCommand.Description = "Unity Web Browser (UWB) Engine - Dedicated process for rendering with a browser engine.";
        
        //Some browser engines will launch multiple processes from the same process, they will most likely use custom arguments
        rootCommand.TreatUnmatchedTokensAsErrors = false;
    }
    
    public int Run(string[] args, Func<LaunchArguments, int> onRun)
    {
        rootCommand.SetAction(result =>
        {
            LaunchArguments arguments = new()
            {
                InitialUrl = result.GetValue(initialUrl),

                Width = result.GetValue(width),
                Height = result.GetValue(height),

                WindowlessFrameRate = result.GetValue(windowlessFrameRate),
                JavaScript = result.GetValue(javaScript),
                WebRtc = result.GetValue(webRtc),
                LocalStorage = result.GetValue(localStorage),
                RemoteDebuggingAllowedOrigins = result.GetValue(remoteDebuggingAllowedOrigins),
                RemoteDebugging = result.GetValue(remoteDebugging),
                IncognitoMode = result.GetValue(incognitoMode),
                CachePath = result.GetValue(cachePath),
                PopupAction = result.GetValue(popupAction),
                NoSandbox = result.GetValue(noSandbox),

                BackgroundColor = new Color(result.GetValue(backgroundColor)),

                ProxyEnabled = result.GetValue(proxyServer),
                ProxyUsername = result.GetValue(proxyUsername),
                ProxyPassword = result.GetValue(proxyPassword),
                
                IgnoreSslErrors = result.GetValue(ignoreSslErrors),
                IgnoreSslErrorsDomains = result.GetValue(ignoreSslErrorsDomains),
                
                LogPath = result.GetValue(logPath),
                LogSeverity = result.GetValue(logSeverity),

                CommunicationLayerName = result.GetValue(communicationLayerName),
                InLocation = result.GetValue(inLocation),
                OutLocation = result.GetValue(outLocation),

                StartDelay = result.GetValue(startDelay)
            };
            
            return onRun.Invoke(arguments);
        });

        return rootCommand.Parse(args).Invoke();
    }
}