using System;
using System.CommandLine;
using System.CommandLine.Invocation;
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
				new Option<int>("-port",
					() => 5555,
					"IPC port"),
				new Option<bool>("-javascript",
					() => true,
					"Enable or disable javascript"),
				new Option<bool>("-debug", 
					() => false,
					"Use debug logging?")
			};
			rootCommand.Description = "Process for windowless CEF rendering.";
			//CEF launches the same process multiple times for its sub-process and passes args to them, so we need to ignore unknown tokens
			rootCommand.TreatUnmatchedTokensAsErrors = false;
			rootCommand.Handler = CommandHandler.Create<string, int, int, byte, byte, byte, byte, int, bool, bool>(
				(initialUrl, width, height, bcr, bcg, bcb, bca, port, javaScript, debug) =>
			{
				Logger.DebugLog = debug;
				CefBrowserProcess browserProcess = null;
				try
				{
					browserProcess = new CefBrowserProcess(initialUrl, width, height,
						new CefColor(bca, bcr, bcg, bcb), port, javaScript, args);
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
	}
}