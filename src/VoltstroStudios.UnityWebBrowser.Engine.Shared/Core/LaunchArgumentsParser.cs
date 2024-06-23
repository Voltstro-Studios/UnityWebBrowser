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
    private readonly LaunchArgumentsBinder launchArgumentsBinder;
    
    public LaunchArgumentsParser()
    {
        //Url to start with
        Option<string> initialUrl = new("-initial-url",
            () => "https://voltstro.dev",
            "The initial URL that the browser will first load to");

        //Resolution
        Option<int> width = new("-width",
            () => 1920,
            "The width of the window");
        Option<int> height = new("-height",
            () => 1080,
            "The height of the window");

        //General browser settings
        Option<bool> javaScript = new("-javascript",
            () => true,
            "Enable or disable javascript");
        Option<bool> webRtc = new("-web-rtc",
            () => false,
            "Enable or disable web RTC");
        Option<bool> localStorage = new("-local-storage",
            () => true,
            "Enable or disable local storage");
        Option<int> remoteDebugging = new("-remote-debugging",
            () => 0,
            "If the engine has remote debugging, what port to use (0 for disable)");
        Option<string[]> remoteDebuggingAllowedOrigins = new("-remote-debugging-allowed-origins",
                        () => new []{"http://127.0.0.1:9022"},
                "Allowed origins for remote debugging.");
        Option<FileInfo> cachePath = new("-cache-path",
            () => null,
            "The path to the cache (null for no cache)");
        Option<PopupAction> popupAction = new("-popup-action", 
            () => PopupAction.Ignore,
            "What action to take when dealing with a popup");

        //Background color
        Option<string> backgroundColor = new("-background-color",
            () => "ffffffff",
            "The color to use for the background");

        //Proxy settings
        Option<bool> proxyServer = new("-proxy-server",
            () => true,
            "Use a proxy server or direct connect");
        Option<string> proxyUsername = new("-proxy-username",
            () => null,
            "The username to use in the proxy auth");
        Option<string> proxyPassword = new("-proxy-password",
            () => null,
            "The password to use in the proxy auth");
        
        //Logging
        Option<FileInfo> logPath = new("-log-path",
            () => new FileInfo("engine.log"),
            "The path to where the log file will be");
        Option<LogSeverity> logSeverity = new("-log-severity",
            () => LogSeverity.Info,
            "The severity of the logs");

        //IPC settings
        Option<FileInfo> communicationLayerPath = new("-comms-layer-path",
            () => null,
            "The location of where the dll for the communication layer is. If none is provided then the in-built TCP layer will be used.");
        Option<string> inLocation = new("-in-location",
            () => "5555",
            "In location for IPC (Pipes location or TCP port in TCP mode)");
        Option<string> outLocation = new("-out-location",
            () => "5556",
            "Out location for IPC (Pipes location or TCP port in TCP mode)");

        //Debugging
        Option<uint> startDelay = new("-start-delay",
            () => 0,
            "Delays the starting process. Used for testing reasons.");

        rootCommand = new RootCommand()
        {
            initialUrl,
            width,
            height,
            javaScript,
            webRtc,
            localStorage,
            remoteDebugging,
            remoteDebuggingAllowedOrigins,
            cachePath,
            popupAction,
            backgroundColor,
            proxyServer,
            proxyUsername,
            proxyPassword,
            logPath,
            logSeverity,
            communicationLayerPath,
            inLocation,
            outLocation,
            startDelay
        };
        rootCommand.Description =
            "Unity Web Browser (UWB) Engine - Dedicated process for rendering with a browser engine.";
        //Some browser engines will launch multiple processes from the same process, they will most likely use custom arguments
        rootCommand.TreatUnmatchedTokensAsErrors = false;
        
        launchArgumentsBinder = new LaunchArgumentsBinder(
            initialUrl,
            width,
            height,
            javaScript,
            webRtc,
            localStorage,
            remoteDebugging,
            remoteDebuggingAllowedOrigins,
            cachePath,
            popupAction,
            backgroundColor,
            proxyServer,
            proxyUsername,
            proxyPassword,
            logPath,
            logSeverity,
            communicationLayerPath,
            inLocation,
            outLocation,
            startDelay);
    }
    
    public int Run(string[] args, Action<LaunchArguments> onRun)
    {
        rootCommand.SetHandler(onRun, launchArgumentsBinder);
        return rootCommand.Invoke(args);
    }
}