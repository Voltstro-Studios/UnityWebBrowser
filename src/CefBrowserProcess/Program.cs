using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Xilium.CefGlue;

namespace CefBrowserProcess
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			RootCommand rootCommand = new RootCommand
			{
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
				new Option<FileInfo>("-log-path", 
					() => new FileInfo("cef.log"),
					"The path to where the CEF log will be"),
				new Option<FileInfo>("-cache-path", 
					() => null, 
					"The path to the cache (null for no cache)"),
				new Option<int>("-port",
					() => 5555,
					"IPC port"),
				new Option<bool>("-debug", 
					() => false,
					"Use debug logging?")
			};
			rootCommand.Description = "Process for windowless CEF rendering.";
			//CEF launches the same process multiple times for its sub-process and passes args to them, so we need to ignore unknown tokens
			rootCommand.TreatUnmatchedTokensAsErrors = false;
			rootCommand.Handler = CommandHandler.Create<Arguments>(parsedArgs =>
			{
				Logger.DebugLog = parsedArgs.Debug;
				CefBrowserProcess browserProcess = null;
				try
				{
					browserProcess = new CefBrowserProcess(parsedArgs.InitialUrl, parsedArgs.Width, parsedArgs.Height,
						new CefColor(parsedArgs.Bca, parsedArgs.Bcr, parsedArgs.Bcg, parsedArgs.Bcb),
						parsedArgs.Port, parsedArgs.JavaScript, parsedArgs.LogPath, parsedArgs.CachePath, args);
				}
				catch (Exception)
				{
					browserProcess?.Dispose();
					return;
				}
				
				browserProcess.HandelEventsLoop();
				browserProcess.Dispose();
			});
			return rootCommand.InvokeAsync(args).Result;
		}

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
			public FileInfo LogPath { get; set; }
			public FileInfo CachePath { get; set; }
			public int Port { get; set; }
			public bool Debug { get; set; }
		}
	}
}