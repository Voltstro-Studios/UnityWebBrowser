using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineActions;
using UnityWebBrowser.Shared.Events.EngineEvents;

namespace UnityWebBrowser.Engine.Shared
{
	/// <summary>
	///		Handles entry stuff for browser engines
	/// </summary>
    public abstract class EngineEntryPoint : IDisposable
	{
		private EventReplier<EngineActionEvent, EngineEvent> eventReplier;
	    
	    /// <summary>
	    ///		Called when the arguments are parsed
	    /// </summary>
	    /// <param name="launchArguments"></param>
	    /// <param name="args"></param>
	    protected abstract void EntryPoint(LaunchArguments launchArguments, string[] args);

	    /// <summary>
	    ///		Called when an event is received
	    /// </summary>
	    /// <param name="actionEvent"></param>
	    /// <returns></returns>
	    protected abstract EngineEvent OnEvent(EngineActionEvent actionEvent);

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
				
				new Option<int>("-port",
					() => 5555,
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
				
				//We might get disposed here
				if(isDisposed)
					return;
				
				//Setup the events replier
				eventReplier = new EventReplier<EngineActionEvent, EngineEvent>(parsedArgs.Port, OnEvent);
				eventReplier.HandleEventsLoop();
				eventReplier.Dispose();
			});
			//Invoke the command line parser and start the handler (the stuff above)
			return rootCommand.Invoke(args);
        }

	    #region Destroy

	    private bool isDisposed;

	    public virtual void Dispose()
	    {
		    eventReplier?.Dispose();
		    isDisposed = true;
		    GC.SuppressFinalize(this);
	    }

	    #endregion
    }
}