// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Threading;
using System.Threading.Tasks;
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

        LaunchArgumentsParser launchArgumentsParser = new();
        return launchArgumentsParser.Run(args, parsedArgs =>
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