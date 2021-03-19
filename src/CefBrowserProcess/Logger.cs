using System;
using System.IO;
using CefBrowserProcess.CommandLine;

namespace CefBrowserProcess
{
	public static class Logger
	{
		[CommandLineArgument("debug")] public static bool DebugLog = false;

		private static readonly TextWriter OutWriter = Console.Out;

		public static void Debug(string message)
		{
			if(DebugLog)
				OutWriter.WriteLine($"[Debug] {message}");
		}

		public static void Info(string message)
		{
			OutWriter.WriteLine($"[Info] {message}");
		}

		public static void Warn(string message)
		{
			OutWriter.WriteLine($"[Warn] {message}");
		}

		public static void Error(string message)
		{
			OutWriter.WriteLine($"[Error] {message}");
		}
	}
}
