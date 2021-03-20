using System;
using CefBrowserProcess.CommandLine;
using CefBrowserProcess.EventData;
using UnityWebBrowser.EventData;
using Xilium.CefGlue;
using ZeroMQ;

namespace CefBrowserProcess
{
	public static class Program
	{
		[CommandLineArgument("url")] public static string InitialUrl = "https://google.com";
		[CommandLineArgument("width")] public static int Width = 1920;
		[CommandLineArgument("height")] public static int Height = 1080;

		[CommandLineArgument("bcr")] public static int BackgroundCRed = 255;
		[CommandLineArgument("bcg")] public static int BackgroundCGreen = 255;
		[CommandLineArgument("bcb")] public static int BackgroundCBlue = 255;
		[CommandLineArgument("bca")] public static int BackgroundCAlpha = 255;

		[CommandLineArgument("port")] public static int Port = 5555;

		public static void Main(string[] args)
		{
			CommandLineParser.Init(args);

			//Setup CEF
			CefRuntime.Load();
			CefMainArgs cefMainArgs = new CefMainArgs(args);
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
				BackgroundColor = new CefColor((byte)BackgroundCAlpha, (byte)BackgroundCRed, (byte)BackgroundCGreen, (byte)BackgroundCBlue),
				JavaScript = CefState.Enabled,
				LocalStorage = CefState.Disabled
			};

			Logger.Info("Starting CEF client...");

			//Create cef browser
			OffscreenCEFClient cefClient = new OffscreenCEFClient(new CefSize(Width, Height));
			CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, InitialUrl);

			//Setup ZMQ
			using ZContext context = new ZContext();
			using ZSocket responder = new ZSocket(context, ZSocketType.REP);

			responder.Bind($"tcp://127.0.0.1:{Port}");

			while (true)
			{
				using ZFrame request = responder.ReceiveFrame();
				string json = request.ReadString();

				//Parse the data we get, and process it
				Logger.Debug(json);
				try
				{
					IEventData data = EventDataParser.ReadData(json);

					if (data.EventType == EventType.Shutdown)
					{
						Logger.Debug("Got shutdown message...");
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
					else if(data.EventType == EventType.LoadHtmlEvent)
						cefClient.LoadHtml((data as LoadHtmlEvent)?.Html);

					responder.Send(new ZFrame((int) EventType.Ping));
				}
				catch (Exception ex)
				{
					Logger.Error($"An exception occurred in the CEF process! {ex.Message}");
					break;
				}
			}

			//Shutdown
			cefClient.Dispose();
			CefRuntime.Shutdown();
		}
	}
}