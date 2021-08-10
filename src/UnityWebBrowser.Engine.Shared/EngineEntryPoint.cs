using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.ReadWriters;
using VoltRpc.Communication;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;
using VoltRpc.Proxy.Generated;

namespace UnityWebBrowser.Engine.Shared
{
	/// <summary>
	///		Handles entry stuff for browser engines
	/// </summary>
    public abstract class EngineEntryPoint : IDisposable
	{
		private const string ActiveEngineFileName = "EngineActive";

		/// <summary>
		///		Fire events on the client end
		/// </summary>
		protected IClient clientEvents;
		
		private Host ipcHost;
		private Client ipcClient;
		
		private FileStream activeFileStream;

		protected bool IsConnected => ipcClient.IsConnected;
		
		/// <summary>
	    ///		Called when the arguments are parsed.
	    ///		<para>Remember to lock if you don't want to immediately exit</para>
	    /// </summary>
	    /// <param name="launchArguments">Arguments as a <see cref="LaunchArguments"/></param>
	    /// <param name="args">
	    ///		Raw arguments inputted.
	    ///		<para>
	    ///			Should only need this if you start up multiple processes.
	    ///		</para>
	    /// </param>
	    protected abstract void EntryPoint(LaunchArguments launchArguments, string[] args);

	    /// <summary>
	    ///		Call this in your engine's Program.Main method.
	    /// </summary>
	    /// <param name="args"></param>
	    /// <returns></returns>
        public int Main(string[] args)
        {
	        RootCommand rootCommand = new RootCommand
			{
				//We got a lot of arguments
				new Option<string>("-initial-url",
					() => "https://voltstro.dev",
					"The initial URL"),
				
				new Option<int>("-width",
					() => 1920,
					"The width of the window"),
				new Option<int>("-height",
					() => 1080,
					"The height of the window"),
				
				new Option<bool>("-javascript",
					() => true,
					"Enable or disable javascript"),
				
				new Option<bool>("-web-rtc",
					() => false,
					"Enable or disable web RTC"),
				
				new Option<int>("-remote-debugging",
					() => 0,
					"Some browser engines may have remote debugging"),
				
				new Option<byte>("-bcr",
					() => 255,
					"Background color (red)"),
				new Option<byte>("-bcg",
					() => 255,
					"Background color (green)"),
				new Option<byte>("-bcb",
					() => 255,
					"Background color (blue)"),
				new Option<byte>("-bca",
					() => 255,
					"Background color (alpha)"),
				
				new Option<FileInfo>("-cache-path", 
					() => null,
					"The path to the cache (null for no cache)"),
				
				new Option<bool>("-proxy-server", 
					() => true,
					"Use a proxy server or direct connect"),
				new Option<string>("-proxy-username",
					() => null,
					"The username to use in proxy auth"),
				new Option<string>("-proxy-password",
					() => null, 
					"The proxy auth password"),
				
				new Option<bool>("-pipes",
					() => true,
					"Use pipes or not"),
				new Option<string>("-in-location",
					() => "UnityWebBrowserIn",
					"In location"),
				new Option<string>("-out-location",
					() => "UnityWebBrowserOut",
					"Out location"),

				new Option<FileInfo>("-log-path", 
					() => new FileInfo("cef.log"),
					"The path to where the log will be"),
				new Option<LogSeverity>("-log-severity", 
					() => LogSeverity.Info,
					"The severity of the logs"),
				
				new Option<string>("-active-engine-file-path", 
					() => string.Empty,
					"Path were the active file will be")
			};
			rootCommand.Description = "Process for windowless CEF rendering.";
			//Some browser engines will launch multiple processes from the same process, they will most likely use custom arguments
			rootCommand.TreatUnmatchedTokensAsErrors = false;
			rootCommand.Handler = CommandHandler.Create<LaunchArguments>(parsedArgs =>
			{
				//Is debug log enabled or not
				Logger.DebugLog = parsedArgs.LogSeverity == LogSeverity.Debug;
				
				//Run the entry point
				try
				{
					EntryPoint(parsedArgs, args);
				}
				catch (Exception ex)
				{
					Logger.ErrorException(ex, "Uncaught exception occured in the entry point!");
					Environment.Exit(-1);
				}
			});
			//Invoke the command line parser and start the handler (the stuff above)
			return rootCommand.Invoke(args);
        }

	    protected void SetupIpc(IEngine engine, LaunchArguments arguments)
	    {
		    try
		    {
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
				    IPEndPoint hostIp = new IPEndPoint(IPAddress.Loopback, inPort);
				    ipcHost = new TCPHost(hostIp);
				    
				    if (!int.TryParse(arguments.OutLocation, out int outPort))
				    {
					    Logger.Error("The provided out port is not an int!");
					    
					    Dispose();
					    return;
				    }
				    
				    Logger.Debug($"Using TCP client on pipe: '{outPort}'");
				    IPEndPoint clientIp = new IPEndPoint(IPAddress.Loopback, outPort);
				    ipcClient = new TCPClient(clientIp);
			    }

			    //IPEndPoint hostIp = new(IPAddress.Loopback, arguments.InPort);
			    //ipcHost = new TCPHost(hostIp);
			    ReadWriterUtils.AddTypeReadWriters(ipcHost.TypeReaderWriterManager);
			    ipcHost.AddService<IEngine>(engine);
                ipcHost.StartListening();
                
                //IPEndPoint clientIp = new(IPAddress.Loopback, arguments.OutPort);
                //ipcClient = new TCPClient(clientIp);
                ReadWriterUtils.AddTypeReadWriters(ipcClient.TypeReaderWriterManager);
                ipcClient.AddService<IClient>();
                Task.Run(() =>
                {
	                try
	                {
		                ipcClient.Connect();
	                }
	                catch (ConnectionFailed)
	                {
		                Logger.Error("The engine failed to connect back to the Unity client! Client events will not fire!");
		                ipcClient.Dispose();
		                ipcClient = null;
	                }
                });
                clientEvents = new ClientProxy(ipcClient);

	            Logger.Debug("IPC Setup done.");
		    }
		    catch (Exception ex)
		    {
			    Logger.ErrorException(ex, "Error setting up IPC!");
		    }
	    }

	    protected void Ready(LaunchArguments arguments)
	    {
		    string path = Path.GetFullPath($"{arguments.ActiveEngineFilePath}/{ActiveEngineFileName}");

		    activeFileStream = File.Create(path, 12, FileOptions.DeleteOnClose);
		    File.SetAttributes(path, FileAttributes.Hidden);
	    }

	    public virtual void Dispose()
	    {
		    ReleaseResources();
		    GC.SuppressFinalize(this);
	    }

	    private void ReleaseResources()
	    {
		    if (activeFileStream != null)
		    {
			    activeFileStream.Close();
			    activeFileStream.Dispose();
		    }
            ipcHost?.Dispose();
	    }
	}
}