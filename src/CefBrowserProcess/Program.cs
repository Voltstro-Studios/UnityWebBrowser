using System;
using CefBrowserProcess.CommandLine;
using CefBrowserProcess.EventData;
using UnityWebBrowser;
using Xilium.CefGlue;
using ZeroMQ;

namespace CefBrowserProcess
{
	public static class Program
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

			//Init CEF and create windowless window
			CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

			CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
			cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

			CefBrowserSettings cefBrowserSettings = new CefBrowserSettings
			{
				BackgroundColor = new CefColor(255, 60, 85, 115),
				JavaScript = CefState.Enabled,
				LocalStorage = CefState.Disabled
			};

			//Create cef browser
			OffscreenCEFClient cefClient = new OffscreenCEFClient(new CefSize(Width, Height));
			CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, InitialUrl);

			//Setup ZMQ
			using ZContext context = new ZContext();
			using ZSocket responder = new ZSocket(context, ZSocketType.REP);
			responder.Bind("tcp://*:5555");

			while (true)
			{
				using ZFrame request = responder.ReceiveFrame();
				string json = request.ReadString();

				//Parse the data we get, and process it
				Console.WriteLine(json);
				try
				{
					IEventData data = EventDataParser.ReadData(json);

					if (data.EventType == EventType.Shutdown)
					{
						Console.WriteLine("Shutting down CEF process...");
						break;
					}

					if(data.EventType == EventType.Ping)
					{
						responder.Send(new ZFrame(cefClient.GetPixels()));
						continue;
					}

					if (data.EventType == EventType.KeyboardEvent)
						cefClient.ProcessKeyboardEvent((KeyboardEvent)data);
					else if (data.EventType == EventType.ButtonEvent)
						cefClient.ProcessButtonEvent((ButtonEvent)data);
					else if (data.EventType == EventType.MouseMoveEvent)
						cefClient.ProcessMouseMoveEvent((MouseMoveEvent)data);
					else if (data.EventType == EventType.MouseClickEvent)
						cefClient.ProcessMouseClickEvent((MouseClickEvent)data);
					else if(data.EventType == EventType.MouseScrollEvent)
						cefClient.ProcessMouseScrollEvent((MouseScrollEvent)data);

					responder.Send(new ZFrame((int) EventType.Ping));
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}

			//Shutdown
			cefClient.Dispose();
			CefRuntime.Shutdown();
		}
	}
}