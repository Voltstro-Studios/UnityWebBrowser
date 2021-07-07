using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Net;
using System.Threading;
using ServiceWire;
using ServiceWire.TcpIp;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;

namespace UnityWebBrowser.Engine.Shared
{
	/// <summary>
	///		Handles entry stuff for browser engines
	/// </summary>
    public abstract class EngineEntryPoint : IDisposable
	{
		private TcpHost ipcHost;
		
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
				
				new Option<int>("-in-port",
					() => 5555,
					"IPC port"),
				new Option<int>("-out-port",
					() => 5556,
					"IPC port"),

				new Option<FileInfo>("-log-path", 
					() => new FileInfo("cef.log"),
					"The path to where the CEF log will be"),
				new Option<LogSeverity>("-log-severity", 
					() => LogSeverity.Info,
					"The path to where the CEF log will be")
			};
			rootCommand.Description = "Process for windowless CEF rendering.";
			//Some browser engines will launch multiple processes from the same process, they will most likely use custom arguments
			rootCommand.TreatUnmatchedTokensAsErrors = false;
			rootCommand.Handler = CommandHandler.Create<LaunchArguments>(parsedArgs =>
			{
				//Is debug log enabled or not
				Logger.DebugLog = parsedArgs.LogSeverity == LogSeverity.Debug;
				
				//Run the entry point
				EntryPoint(parsedArgs, args);

				Console.ReadKey();
			});
			//Invoke the command line parser and start the handler (the stuff above)
			return rootCommand.Invoke(args);
        }

	    protected void SetupIpc(IEngine engine, LaunchArguments arguments)
	    {
		    try
		    {
			    ServiceWire.Logger logger = new ServiceWire.Logger(logLevel: LogLevel.Debug);
			    Stats stats = new Stats();

			    IPEndPoint ip = new IPEndPoint(IPAddress.Any, 5555);

			    ipcHost = new TcpHost(ip, logger, stats);

			    ipcHost.AddService(engine);
			    ipcHost.Open();
			    Logger.Debug("IPC Setup done.");
		    }
		    catch (Exception ex)
		    {
			    Logger.ErrorException(ex, "Error setting up IPC!");
		    }
	    }

	    public virtual void Dispose()
	    {
		    ReleaseResources();
		    GC.SuppressFinalize(this);
	    }

	    private void ReleaseResources()
	    {
		    ipcHost?.Close();
		    ipcHost?.Dispose();
	    }
	}
}