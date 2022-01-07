using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityWebBrowser.Engine.Shared.Core.Logging;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.ReadWriters;
using VoltRpc.Communication;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;

namespace UnityWebBrowser.Engine.Shared
{
	/// <summary>
	///     Handles entry stuff for browser engines
	/// </summary>
	public abstract class EngineEntryPoint : IDisposable
    {
        /// <summary>
        ///     Allows the engine to fire events on the Unity client side
        /// </summary>
        protected ClientActions ClientActions { get; private set; }

        private Client ipcClient;
        private Host ipcHost;

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
            //We got a lot of arguments
            
            //Url to start with
            Option<string> initialUrl = new Option<string>("-initial-url",
                () => "https://voltstro.dev",
                "The initial URL that the browser will first load to");
            
            //Resolution
            Option<int> width = new Option<int>("-width",
                () => 1920,
                "The width of the window");
            Option<int> height = new Option<int>("-height",
                () => 1080,
                "The height of the window");

            //General browser settings
            Option<bool> javaScript = new Option<bool>("-javascript",
                () => true,
                "Enable or disable javascript");
            Option<bool> webRtc = new Option<bool>("-web-rtc",
                () => false,
                "Enable or disable web RTC");
            Option<int> remoteDebugging = new Option<int>("-remote-debugging",
                () => 0,
                "If the engine has remote debugging, what port to use (0 for disable)");
            Option<FileInfo> cachePath = new Option<FileInfo>("-cache-path",
                () => null,
                "The path to the cache (null for no cache)");

            //Background color
            Option<Color> backgroundColor = new Option<Color>("-background-color",
                () => new Color("ffffffff"),
                "The color to use for the background");

            //Proxy settings
            Option<bool> proxyServer = new Option<bool>("-proxy-server",
                () => true,
                "Use a proxy server or direct connect");
            Option<string> proxyUsername = new Option<string>("-proxy-username",
                () => null,
                "The username to use in the proxy auth");
            Option<string> proxyPassword = new Option<string>("-proxy-password",
                () => null,
                "The password to use in the proxy auth");
            
            //IPC settings
            Option<bool> pipes = new Option<bool>("-pipes",
                () => true,
                "Use pipes or TCP");
            Option<string> inLocation = new Option<string>("-in-location",
                () => "UnityWebBrowserIn",
                "In location for IPC (Pipes location or TCP port in TCP mode)");
            Option<string> outLocation = new Option<string>("-out-location",
                () => "UnityWebBrowserOut",
                "Out location for IPC (Pipes location or TCP port in TCP mode)");

            Option<FileInfo> logPath = new Option<FileInfo>("-log-path",
                () => new FileInfo("engine.log"),
                "The path to where the log file will be");
            Option<LogSeverity> logSeverity = new Option<LogSeverity>("-log-severity",
                () => LogSeverity.Info,
                "The severity of the logs");

            RootCommand rootCommand = new()
            {
                initialUrl,
                width, height,
                javaScript, webRtc, remoteDebugging, cachePath,
                backgroundColor,
                proxyServer, proxyUsername, proxyPassword, 
                pipes, inLocation, outLocation, 
                logPath, logSeverity
            };
            rootCommand.Description = "Unity Web Browser (UWB) Engine - Dedicated process for rendering with a browser engine.";
            //Some browser engines will launch multiple processes from the same process, they will most likely use custom arguments
            rootCommand.TreatUnmatchedTokensAsErrors = false;

            //The new version of System.CommandLine is very boiler platey
            LaunchArgumentsBinder launchArgumentBinder = new(
                initialUrl,
                width, height,
                javaScript, webRtc, remoteDebugging, cachePath,
                backgroundColor,
                proxyServer, proxyUsername, proxyPassword, 
                pipes, inLocation, outLocation, 
                logPath, logSeverity);
            rootCommand.SetHandler((LaunchArguments parsedArgs) =>
            {
                if(ShouldInitLogger(parsedArgs, args))
                    Logger.Init(parsedArgs.LogSeverity);
                
                //Is debug log enabled or not
                ClientActions = new ClientActions();

                //Run early init
                try
                {
                    EarlyInit(parsedArgs, args);
                }
                catch (Exception)
                {
                    Environment.Exit(-1);
                    return;
                }

                //Run the entry point
                try
                {
                    EntryPoint(parsedArgs, args);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Uncaught exception occured in the entry point!");
#if DEBUG
                    Debugger.Break();
#endif
                    Environment.Exit(-1);
                }
                
                Logger.Shutdown();
            }, launchArgumentBinder);

            //Invoke the command line parser and start the handler (the stuff above)
            return rootCommand.Invoke(args);
        }

        /// <summary>
        ///     Call when you are ready to setup the IPC
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="arguments"></param>
        protected void SetupIpc(IEngine engine, LaunchArguments arguments)
        {
            try
            {
                //Setup IPC, if we are pipes then we use the PipesHost/PipesClient, otherwise TCP
                if (arguments.Pipes)
                {
                    Logger.Debug("Using pipes host on pipe: {InLocation}", arguments.InLocation);
                    ipcHost = new PipesHost(arguments.InLocation);

                    Logger.Debug("Using pipes client on pipe: {OutLocation}", arguments.OutLocation);
                    ipcClient = new PipesClient(arguments.OutLocation);
                }
                else
                {
                    if (!int.TryParse(arguments.InLocation, out int inPort))
                    {
                        Logger.Error("The provided in port is not an int!");

                        Dispose();
                        return;
                    }
                    Logger.Debug("Using TCP host on port: {InLocation}", inPort);
                    
                    IPEndPoint hostIp = new(IPAddress.Loopback, inPort);
                    ipcHost = new TCPHost(hostIp);

                    if (!int.TryParse(arguments.OutLocation, out int outPort))
                    {
                        Logger.Error("The provided out port is not an int!");

                        Dispose();
                        return;
                    }
                    Logger.Debug($"Using TCP client on port: {outPort}");
                    
                    IPEndPoint clientIp = new(IPAddress.Loopback, outPort);
                    ipcClient = new TCPClient(clientIp);
                }
                
                ReadWriterUtils.AddTypeReadWriters(ipcHost.TypeReaderWriterManager);
                ipcHost.AddService(typeof(IEngine), engine);
                ipcHost.StartListening();
                
                ReadWriterUtils.AddTypeReadWriters(ipcClient.TypeReaderWriterManager);
                ipcClient.AddService(typeof(IClient));
                
                //Connect the server (us) back to Unity
                try
                {
                    ipcClient.Connect();
                    ClientActions.SetIpcClient(ipcClient);
                }
                catch (ConnectionFailedException)
                {
                    Logger.Error(
                        "The engine failed to connect back to the Unity client! Client events will not fire!");
                    ipcClient.Dispose();
                    ipcClient = null;
                }

                Logger.Debug("IPC Setup done.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error setting up IPC!");
            }
        }

        /// <summary>
        ///     Call when you are ready
        /// </summary>
        protected void Ready()
        {
            ClientActions.Ready();
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
            ClientActions.Dispose();
            ipcHost?.Dispose();
        }

        #endregion
    }
}