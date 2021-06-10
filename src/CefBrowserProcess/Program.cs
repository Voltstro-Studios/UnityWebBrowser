using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using CefBrowserProcess.Core;
using CefBrowserProcess.Models;
using UnityWebBrowser.Shared;

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

				//Create CefBrowserProcess class, which is responsible for basically everything
				Core.CefBrowserProcess browserProcess = null;
				try
				{
					//Create it with our parsed arguments
					browserProcess = new Core.CefBrowserProcess(parsedArgs, args);
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
	}
}