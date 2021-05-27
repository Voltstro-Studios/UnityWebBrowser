using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using CefBrowserProcess.Core;
using CefBrowserProcess.Models;
using Xilium.CefGlue;

namespace CefBrowserProcess
{
	/// <summary>
	///		Main class for this program
	/// </summary>
	public static class Program
	{
		/// <summary>
		///		Entry point
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static int Main(string[] args)
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
				new Option<string>("-proxy-username",
					() => null,
					"The username to use in proxy auth"),
				new Option<string>("-proxy-password",
					() => null, 
					"The proxy auth password"),
				new Option<int>("-port",
					() => 5555,
					"IPC port"),
				new Option<bool>("-debug", 
					() => false,
					"Use debug logging?"),
				new Option<FileInfo>("-log-path", 
					() => new FileInfo("cef.log"),
					"The path to where the CEF log will be"),
				new Option<CefLogSeverity>("-log-severity", 
					() => CefLogSeverity.Default,
					"The path to where the CEF log will be"),
			};
			rootCommand.Description = "Process for windowless CEF rendering.";
			//CEF launches the same process multiple times for its sub-process and passes args to them, so we need to ignore unknown tokens
			rootCommand.TreatUnmatchedTokensAsErrors = false;
			rootCommand.Handler = CommandHandler.Create<Arguments>(parsedArgs =>
			{
				//Is debug log enabled or not
				Logger.DebugLog = parsedArgs.Debug;

				//Create CefBrowserProcess class, which is responsible for basically everything
				Core.CefBrowserProcess browserProcess = null;
				try
				{
					//Create it with our parsed arguments
					browserProcess = new Core.CefBrowserProcess(parsedArgs.InitialUrl, parsedArgs.Width, parsedArgs.Height,
						new CefColor(parsedArgs.Bca, parsedArgs.Bcr, parsedArgs.Bcg, parsedArgs.Bcb),
						parsedArgs.Port, parsedArgs.JavaScript, parsedArgs.LogPath, parsedArgs.LogSeverity, parsedArgs.CachePath, new ProxySettings
						{
							Username = parsedArgs.ProxyUsername,
							Password = parsedArgs.ProxyPassword
						}, args);
				}
				catch (Exception)
				{
					browserProcess?.Dispose();
					Environment.Exit(0);
					return;
				}
				
				//Start our events loop
				browserProcess.HandelEventsLoop();
				
				//The end
				browserProcess.Dispose();
			});
			//Invoke the command line parser and start the handler (the stuff above)
			return rootCommand.InvokeAsync(args).Result;
		}

		// ReSharper disable UnusedAutoPropertyAccessor.Local
		// ReSharper disable once ClassNeverInstantiated.Local
		private class Arguments
		{
			public string InitialUrl { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public bool JavaScript { get; set; }
			public byte Bcr { get; set; }
			public byte Bcg { get; set; }
			public byte Bcb { get; set; }
			public byte Bca { get; set; }
			public FileInfo CachePath { get; set; }
			public string ProxyUsername { get; set; }
			public string ProxyPassword { get; set; }
			public int Port { get; set; }
			public bool Debug { get; set; }
			public FileInfo LogPath { get; set; }
			public CefLogSeverity LogSeverity { get; set; }
		}
		// ReSharper restore UnusedAutoPropertyAccessor.Local
	}
}