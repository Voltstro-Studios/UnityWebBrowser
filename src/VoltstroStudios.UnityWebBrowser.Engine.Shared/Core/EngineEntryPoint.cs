// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Communications;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;
using VoltRpc.Communication;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Communications;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core.Logging;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Popups;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.ReadWriters;

#if LINUX
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Native.Linux;
#endif

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     Handles entry stuff for browser engines
/// </summary>
internal abstract class EngineEntryPoint : IDisposable
{
    private Client ipcClient;
    private Host ipcHost;

    /// <summary>
    ///     Allows the engine to fire events on the Unity client side
    /// </summary>
    protected ClientControlsActions ClientControlsActions { get; private set; }
    
    /// <summary>
    ///     Call to invoke new popups
    /// </summary>
    protected EnginePopupManager PopupManager { get; private set; }

    /// <summary>
    ///     Is the <see cref="Client" /> side of the connection connected
    /// </summary>
    protected bool IsConnected => ipcClient.IsConnected;

    protected abstract bool ShouldInitLogger(LaunchArguments launchArguments, string[] args);

    /// <summary>
    ///     Do your early init stuff here
    /// </summary>
    protected abstract void EarlyInit(LaunchArguments launchArguments, string[] args);

    /// <summary>
    ///     Called when the arguments are parsed.
    ///     <para>Remember to lock if you don't want to immediately exit</para>
    /// </summary>
    /// <param name="launchArguments">Arguments as a <see cref="LaunchArguments" /></param>
    /// <param name="args">
    ///     Raw arguments inputted.
    ///     <para>
    ///         Should only need this if you start up multiple processes.
    ///     </para>
    /// </param>
    protected abstract void EntryPoint(LaunchArguments launchArguments, string[] args);

    /// <summary>
    ///     Call this in your engine's Program.Main method.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public int Main(string[] args)
    {
        #if LINUX
        //On Linux, tell this child process to kill it self when it's parent process dies
        //Option: 1 -> PR_SET_PDEATHSIG (include/uapi/linux/prctl.h)
        //Arg2: 9 -> SIGKILL (arch/x86/include/uapi/asm/signal.h)
        SysPrctl.prctl(1, 9);
        #endif
        
        //We got a lot of arguments

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

        RootCommand rootCommand = new()
        {
            initialUrl,
            width, height,
            javaScript, webRtc, localStorage, remoteDebugging, cachePath, popupAction,
            backgroundColor,
            proxyServer, proxyUsername, proxyPassword,
            logPath, logSeverity,
            communicationLayerPath, inLocation, outLocation,
            startDelay
        };
        rootCommand.Description =
            "Unity Web Browser (UWB) Engine - Dedicated process for rendering with a browser engine.";
        //Some browser engines will launch multiple processes from the same process, they will most likely use custom arguments
        rootCommand.TreatUnmatchedTokensAsErrors = false;

        //The new version of System.CommandLine is very boiler platey
        LaunchArgumentsBinder launchArgumentBinder = new(
            initialUrl,
            width, height,
            javaScript, webRtc, localStorage, remoteDebugging, cachePath, popupAction,
            backgroundColor,
            proxyServer, proxyUsername, proxyPassword,
            logPath, logSeverity,
            communicationLayerPath, inLocation, outLocation,
            startDelay);
        rootCommand.SetHandler(parsedArgs =>
        {
            if(parsedArgs.StartDelay != 0)
                Thread.Sleep((int)parsedArgs.StartDelay);
            
            if (ShouldInitLogger(parsedArgs, args))
                Logger.Init(parsedArgs.LogSeverity);
            
            ClientControlsActions = new ClientControlsActions();
            PopupManager = new EnginePopupManager();

            //Run early init
            try
            {
                EarlyInit(parsedArgs, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"{Logger.BaseLoggingTag}: Uncaught exception occured in early init!");
                ShutdownAndExitWithError();
                return;
            }

            //Run the entry point
            try
            {
                EntryPoint(parsedArgs, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"{Logger.BaseLoggingTag}: Uncaught exception occured in the entry point!");
                ShutdownAndExitWithError();
                return;
            }

            Logger.Shutdown();
        }, launchArgumentBinder);

        //Invoke the command line parser and start the handler (the stuff above)
        return rootCommand.Invoke(args);
    }

    /// <summary>
    ///     Call when you are ready to setup the IPC
    /// </summary>
    /// <param name="engineControls"></param>
    /// <param name="arguments"></param>
    internal void SetupIpc(IEngineControls engineControls, LaunchArguments arguments)
    {
        try
        {
            Logger.Debug($"{Logger.BaseLoggingTag}: SetupIcp.");
            
            ICommunicationLayer communicationLayer;
            if (arguments.CommunicationLayerPath == null)
            {
                //Use TCP
                Logger.Debug($"{Logger.BaseLoggingTag}: No communication layer provided, using default TCP...");
                communicationLayer = new TCPCommunicationLayer();
                Logger.Debug($"{Logger.BaseLoggingTag}: Created default TCP communication layer.");
            }
            else
            {
                communicationLayer = CommunicationLayerLoader.GetCommunicationLayerFromAssembly(
                    arguments.CommunicationLayerPath.FullName);
            }
            
            Logger.Debug($"{Logger.BaseLoggingTag}: Created communication layer of type '{communicationLayer.GetType().FullName}'...");

            try
            {
                ipcHost = communicationLayer.CreateHost(arguments.InLocation);
                ipcClient = communicationLayer.CreateClient(arguments.OutLocation);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"{Logger.BaseLoggingTag}: An error occured setting up the communication layer!");
                ShutdownAndExitWithError();
                return;
            }
            
            Logger.Debug($"{Logger.BaseLoggingTag}: Created host and client from communication layer.");

            //Add type readers
            EngineReadWritersManager.AddTypeReadWriters(ipcHost.TypeReaderWriterManager);
            ipcHost.AddService(typeof(IEngineControls), engineControls);
            ipcHost.AddService(typeof(IPopupClientControls), PopupManager);
            Logger.Debug($"{Logger.BaseLoggingTag}: Installed services on host.");
            
            Task.Run(() =>
            {
                try
                {
                    ipcHost.StartListening();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"{Logger.BaseLoggingTag}: An error occured listening on host!");
                    ShutdownAndExitWithError();
                }
            });
            
            Logger.Debug($"{Logger.BaseLoggingTag}: Host has started listening.");

            EngineReadWritersManager.AddTypeReadWriters(ipcClient.TypeReaderWriterManager);
            ipcClient.AddService(typeof(IClientControls));
            ipcClient.AddService(typeof(IPopupEngineControls));
            
            Logger.Debug($"{Logger.BaseLoggingTag}: Installed services on client.");

            //Connect the engine (us) back to Unity
            try
            {
                ipcClient.Connect();
                
                Logger.Debug($"{Logger.BaseLoggingTag}: Client has connected back to Unity.");
                
                ClientControlsActions.SetIpcClient(ipcClient);
                PopupManager.SetIpcClient(ipcClient);
            }
            catch (ConnectionFailedException)
            {
                Logger.Error(
                    $"{Logger.BaseLoggingTag}: The engine failed to connect back to the Unity client! Client events will not fire!");
                ipcClient.Dispose();
                ipcClient = null;
            }

            Logger.Debug($"{Logger.BaseLoggingTag}: IPC Setup done.");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"{Logger.BaseLoggingTag}: Error setting up IPC!");
        }
    }

    /// <summary>
    ///     Call when you are ready
    /// </summary>
    protected void Ready()
    {
        ClientControlsActions.Ready();
    }

    private void ShutdownAndExitWithError()
    {
        Dispose();
        Logger.Shutdown();
        Environment.Exit(-1);
    }

    #region Destroy

    ~EngineEntryPoint()
    {
        ReleaseResources();
    }

    /// <summary>
    ///     Destroys this <see cref="EngineEntryPoint" /> instance
    /// </summary>
    public void Dispose()
    {
        ReleaseResources();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Called when <see cref="Dispose" /> is invoked
    /// </summary>
    protected virtual void ReleaseResources()
    {
        ClientControlsActions.Dispose();
        ipcHost?.Dispose();
    }

    #endregion
}