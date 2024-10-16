// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VoltstroStudios.UnityWebBrowser.Shared.Communications;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;
using VoltRpc.Communication;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Communications;
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
    ///     <see cref="LoggerManager"/> for creating <see cref="ILogger{TCategoryName}"/>s
    /// </summary>
    protected LoggerManager LoggerManagerFactory;
    private ILogger engineLogger;

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

        LaunchArgumentsParser launchArgumentsParser = new();
        return launchArgumentsParser.Run(args, parsedArgs =>
        {
            try
            {
                //Start logging services
                LoggerManagerFactory = new LoggerManager(parsedArgs.LogSeverity);
                engineLogger = LoggerManagerFactory.CreateLogger("Engine");
            
                if(parsedArgs.StartDelay != 0)
                    Thread.Sleep((int)parsedArgs.StartDelay);
                
                ClientControlsActions = new ClientControlsActions();
                PopupManager = new EnginePopupManager();
                
                if(parsedArgs.IgnoreSslErrors && parsedArgs.IgnoreSslErrorsDomains != null)
                    engineLogger.LogWarning("Ignore Ssl Errors is enabled! Proceed with caution on these domains: {Domains}", string.Join(", ", parsedArgs.IgnoreSslErrorsDomains));
                
                EarlyInit(parsedArgs, args);
                
                EntryPoint(parsedArgs, args);
            }
            catch (Exception ex)
            {
                engineLogger.LogCritical(ex, "Uncaught exception occured in the entry point!");
                ShutdownAndExitWithError();
            }
        });
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
            engineLogger.LogDebug("Doing IPC Setup...");

            //Create communication layer 
            ICommunicationLayer communicationLayer = arguments.CommunicationLayerName switch
            {
                "TCP" => new TCPCommunicationLayer(),
                "Pipes" => new PipesCommunicationLayer(),
                _ => throw new NullReferenceException("Unknown communication layer!")
            };

            engineLogger.LogDebug("Created communication layer of type '{communicationLayerName}'...", communicationLayer.GetType().FullName);

            try
            {
                ipcHost = communicationLayer.CreateHost(arguments.InLocation);
                ipcClient = communicationLayer.CreateClient(arguments.OutLocation);
            }
            catch (Exception ex)
            {
                engineLogger.LogError(ex, "An error occured setting up the communication layer!");
                ShutdownAndExitWithError();
                return;
            }
            
            engineLogger.LogDebug($"Created host and client from communication layer.");

            //Add type readers
            EngineReadWritersManager.AddTypeReadWriters(ipcHost.TypeReaderWriterManager);
            ipcHost.AddService(typeof(IEngineControls), engineControls);
            ipcHost.AddService(typeof(IPopupClientControls), PopupManager);
            engineLogger.LogDebug("Installed services on host.");
            
            Task.Run(() =>
            {
                try
                {
                    ipcHost.StartListening();
                }
                catch (Exception ex)
                {
                    engineLogger.LogError(ex, "An error occured listening on host!");
                    ShutdownAndExitWithError();
                }
            });
            
            engineLogger.LogDebug("Host has started listening.");

            EngineReadWritersManager.AddTypeReadWriters(ipcClient.TypeReaderWriterManager);
            ipcClient.AddService(typeof(IClientControls));
            ipcClient.AddService(typeof(IPopupEngineControls));
            
            engineLogger.LogDebug("Installed services on client.");

            //Connect the engine (us) back to Unity
            try
            {
                ipcClient.Connect();
                
                engineLogger.LogDebug("Client has connected back to Unity.");
                
                ClientControlsActions.SetIpcClient(ipcClient);
                PopupManager.SetIpcClient(ipcClient);
            }
            catch (ConnectionFailedException)
            {
                engineLogger.LogWarning("The engine failed to connect back to the Unity client! Client events will not fire!");
                ipcClient.Dispose();
                ipcClient = null;
            }

            engineLogger.LogDebug("IPC Setup done.");
        }
        catch (Exception ex)
        {
            engineLogger.LogError(ex, "Error setting up IPC!");
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