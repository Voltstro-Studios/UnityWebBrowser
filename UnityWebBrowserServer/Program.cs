using System;
using Newtonsoft.Json;
using Voltstro.CommandLineParser;
using Xilium.CefGlue;
using ZeroMQ;

namespace UnityWebBrowserServer
{
	public class Program
	{
		[CommandLineArgument("url")] public static string InitialUrl = "https://google.com";
		[CommandLineArgument("width")] public static int Width = 1920;
		[CommandLineArgument("height")] public static int Height = 1080;

		public static void Main(string[] args)
		{
			CommandLineParser.Init(args);
			Console.WriteLine($"Starting CEF with {Width}x{Height} at {InitialUrl}");

			//Setup CEF
			CefRuntime.Load();

			CefMainArgs cefMainArgs = new CefMainArgs(new string[0]);
			OffscreenCEFClient.OffscreenCEFApp cefApp = new OffscreenCEFClient.OffscreenCEFApp();

			CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero);

			CefSettings cefSettings = new CefSettings
			{
				WindowlessRenderingEnabled = true,
				NoSandbox = true,
				LogFile = "cef.log",
				MultiThreadedMessageLoop = true
			};

			CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

			CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
			cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

			CefBrowserSettings cefBrowserSettings = new CefBrowserSettings
			{
				BackgroundColor = new CefColor(255, 60, 85, 115),
				JavaScript = CefState.Enabled,
				LocalStorage = CefState.Disabled
			};

			OffscreenCEFClient cefClient = new OffscreenCEFClient(new CefSize(Width, Height));
			CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, InitialUrl);

			//Setup server
			using ZContext context = new ZContext();
			using ZSocket responder = new ZSocket(context, ZSocketType.REP);
			responder.Bind("tcp://*:5555");

			while (true)
			{
				using ZFrame request = responder.ReceiveFrame();
				string json = request.ReadString();
				Console.WriteLine(json);
				try
				{
					EventData data = JsonConvert.DeserializeObject<EventData>(json);

					if(data.Shutdown)
						break;

					cefClient.ProcessEventData(data);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				
				responder.Send(new ZFrame(cefClient.GetPixels()));
			}

			cefClient.Dispose();
			CefRuntime.Shutdown();
		}
	}
}