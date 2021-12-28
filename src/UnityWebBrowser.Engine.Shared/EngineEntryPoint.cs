using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
        private const string ActiveEngineFileName = "EngineActive";

        private FileStream activeFileStream;

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
                "The initial URL");
            
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
                "Some browser engines may have remote debugging");
            Option<FileInfo> cachePath = new Option<FileInfo>("-cache-path",
                () => null,
                "The path to the cache (null for no cache)");

            //Background color
            Option<byte> bcr = new Option<byte>("-bcr",
                () => 255,
                "Background color (red)");
            Option<byte> bcg = new Option<byte>("-bcg",
                () => 255,
                "Background color (green)");
            Option<byte> bcb = new Option<byte>("-bcb",
                () => 255,
                "Background color (blue)");
            Option<byte> bca = new Option<byte>("-bca",
                () => 255,
                "Background color (alpha)");

            //Proxy settings
            Option<bool> proxyServer = new Option<bool>("-proxy-server",
                () => true,
                "Use a proxy server or direct connect");
            Option<string> proxyUsername = new Option<string>("-proxy-username",
                () => null,
                "The username to use in proxy auth");
            Option<string> proxyPassword = new Option<string>("-proxy-password",
                () => null,
                "The proxy auth password");
            
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
                "The path to where the log will be");
            Option<LogSeverity> logSeverity = new Option<LogSeverity>("-log-severity",
                () => LogSeverity.Info,
                "The severity of the logs");

            Option<string> activeEngineFilePath = new Option<string>("-active-engine-file-path",
                () => AppContext.BaseDirectory,
                "Path were the active file will be");

            RootCommand rootCommand = new()
            {
                initialUrl,
                width, height,
                javaScript, webRtc, remoteDebugging, cachePath,
                bcr, bcg, bcb, bca,
                proxyServer, proxyUsername, proxyPassword, 
                pipes, inLocation, outLocation, 
                logPath, logSeverity,
                activeEngineFilePath
            };
            rootCommand.Description = "Headless browser engine renderer. Communication is done over IPC.";
            //Some browser engines will launch multiple processes from the same process, they will most likely use custom arguments
            rootCommand.TreatUnmatchedTokensAsErrors = false;
            
            //The new version of System.CommandLine is very boiler platey
            LaunchArgumentsBinder launchArgumentBinder = new(initialUrl,
                width, height,
                javaScript, webRtc, remoteDebugging, cachePath,
                bcr, bcg, bcb, bca,
                proxyServer, proxyUsername, proxyPassword, 
                pipes, inLocation, outLocation, 
                logPath, logSeverity,
                activeEngineFilePath);
            rootCommand.SetHandler((LaunchArguments parsedArgs) =>
            {
                //Is debug log enabled or not
                Logger.DebugLog = parsedArgs.LogSeverity == LogSeverity.Debug;
                ClientActions = new ClientActions();

                //Run the entry point
                try
                {
                    EntryPoint(parsedArgs, args);
                }
                catch (Exception ex)
                {
                    Logger.ErrorException(ex, "Uncaught exception occured in the entry point!");
#if DEBUG
                    Debugger.Break();
#endif
                    Environment.Exit(-1);
                }
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
                    Logger.Debug($"Using pipes host on pipe: '{arguments.InLocation}'");
                    ipcHost = new PipesHost(arguments.InLocation);

                    Logger.Debug($"Using pipes client on pipe: '{arguments.OutLocation}'");
                    ipcClient = new PipesClient(arguments.OutLocation);
                }
                else
                {
                    Logger.Debug($"Using pipes host on pipe: '{arguments.InLocation}'");
                    if (!int.TryParse(arguments.InLocation, out int inPort))
                    {
                        Logger.Error("The provided in port is not an int!");

                        Dispose();
                        return;
                    }

                    Logger.Debug($"Using TCP host port: {inPort}");
                    IPEndPoint hostIp = new(IPAddress.Loopback, inPort);
                    ipcHost = new TCPHost(hostIp);

                    if (!int.TryParse(arguments.OutLocation, out int outPort))
                    {
                        Logger.Error("The provided out port is not an int!");

                        Dispose();
                        return;
                    }

                    Logger.Debug($"Using TCP client port: {outPort}");
                    IPEndPoint clientIp = new(IPAddress.Loopback, outPort);
                    ipcClient = new TCPClient(clientIp);
                }
                
                ReadWriterUtils.AddTypeReadWriters(ipcHost.TypeReaderWriterManager);
                ipcHost.AddService(typeof(IEngine), engine);
                ipcHost.StartListening();
                
                ReadWriterUtils.AddTypeReadWriters(ipcClient.TypeReaderWriterManager);
                ipcClient.AddService(typeof(IClient));
                Task.Run(() =>
                {
                    //Connect the server (us) back to Unity
                    try
                    {
                        ipcClient.Connect();
                    }
                    catch (ConnectionFailed)
                    {
                        Logger.Error(
                            "The engine failed to connect back to the Unity client! Client events will not fire!");
                        ipcClient.Dispose();
                        ipcClient = null;
                    }
                });
                ClientActions.SetIpcClient(ipcClient);

                Logger.Debug("IPC Setup done.");
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, "Error setting up IPC!");
            }
        }

        /// <summary>
        ///     Call when you are ready
        /// </summary>
        /// <param name="arguments"></param>
        protected void Ready(LaunchArguments arguments)
        {
            string path = Path.GetFullPath($"{arguments.ActiveEngineFilePath}/{ActiveEngineFileName}");

            activeFileStream = File.Create(path, 12, FileOptions.DeleteOnClose);
            File.SetAttributes(path, FileAttributes.Hidden);
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
            if (activeFileStream != null)
            {
                activeFileStream.Close();
                activeFileStream.Dispose();
            }

            ipcHost?.Dispose();
        }

        #endregion
    }
}