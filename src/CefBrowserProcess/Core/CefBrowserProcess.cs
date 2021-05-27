using System;
using System.IO;
using CefBrowserProcess.Browser;
using CefBrowserProcess.EventData;
using CefBrowserProcess.Models;
using UnityWebBrowser.EventData;
using Xilium.CefGlue;
using ZeroMQ;

namespace CefBrowserProcess.Core
{
	/// <summary>
	///		Main class responsible for managing CEF and the events
	/// </summary>
	public class CefBrowserProcess : IDisposable
	{
		private const int EventPassingNumErrorsAllowed = 4;

		private readonly int ipcPort;
		private readonly BrowserProcessCEFClient cefClient;

		private bool isRunning;

		///  <summary>
		/// 		Creates a new <see cref="CefBrowserProcess"/> instance
		///  </summary>
		///  <param name="initialUrl"></param>
		///  <param name="width"></param>
		///  <param name="height"></param>
		///  <param name="backgroundColor"></param>
		///  <param name="port"></param>
		///  <param name="javaScript"></param>
		///  <param name="logPath"></param>
		///  <param name="logSeverity"></param>
		///  <param name="cachePath"></param>
		///  <param name="proxySettings"></param>
		///  <param name="cefArgs"></param>
		///  <exception cref="Exception"></exception>
		public CefBrowserProcess(string initialUrl, int width, int height, CefColor backgroundColor, int port, bool javaScript, 
			FileSystemInfo logPath, CefLogSeverity logSeverity, FileSystemInfo cachePath, ProxySettings proxySettings, string[] cefArgs)
		{
			ipcPort = port;

			//Setup CEF
			try
			{
				CefRuntime.Load();
			}
			catch (DllNotFoundException)
			{
				Logger.Error("Failed to load the CEF runtime as the required CEF libs are missing!");
				throw new Exception();
			}
			catch (CefVersionMismatchException)
			{
				Logger.Error(
					$"Failed to load the CEF runtime as the installed CEF libs are incompatible! Expected version CEF version: {CefRuntime.ChromeVersion}.");
				throw new Exception();
			}
			catch (Exception ex)
			{
				Logger.ErrorException(ex, "Failed to load the CEF runtime for some reason!");
				throw new Exception();
			}
			
			//Do we have a cache or not, if not CEF will run in "incognito" mode.
			string cachePathArgument = null;
			if (cachePath != null)
				cachePathArgument = cachePath.FullName;

			//Setup the CEF settings
			CefSettings cefSettings = new CefSettings
			{
				WindowlessRenderingEnabled = true,
				NoSandbox = true,
				LogFile = logPath.FullName,
				CachePath = cachePathArgument,
				MultiThreadedMessageLoop = true,
				LogSeverity = logSeverity,
				Locale = "en-US",
				ExternalMessagePump = false,
#if LINUX
				//On Linux we need to tell CEF where everything is
				ResourcesDirPath = Path.Combine(Environment.CurrentDirectory),
				LocalesDirPath = Path.Combine(Environment.CurrentDirectory, "locales"),
				BrowserSubprocessPath = Path.Combine(Environment.CurrentDirectory, "cefsimple")
#endif
			};
			
			//Set up CEF args and the CEF app
			CefMainArgs cefMainArgs = new CefMainArgs(cefArgs);
			BrowserProcessCEFApp cefApp = new BrowserProcessCEFApp();
#if WINDOWS
			//Run our sub-processes
			int exitCode = CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero);
			if (exitCode != -1)
				throw new Exception();
#endif

			//Init CEF
			CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

			//Create a CEF window and set it to windowless
			CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
			cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

			//Create our CEF browser settings
			CefBrowserSettings cefBrowserSettings = new CefBrowserSettings
			{
				BackgroundColor = backgroundColor,
				JavaScript = javaScript ? CefState.Enabled : CefState.Disabled,
				LocalStorage = CefState.Disabled
			};

			Logger.Debug($"CEF starting with these options:" +
			             $"\nJS: {javaScript}" +
			             $"\nBackgroundColor: {backgroundColor}" +
			             $"\nCache Path: {cachePathArgument}" +
			             $"\nLog Path: {logPath.FullName}" +
			             $"\nLog Severity: {logSeverity}");
			Logger.Info("Starting CEF client...");

			//Create cef browser
			try
			{
				cefClient = new BrowserProcessCEFClient(new CefSize(width, height), proxySettings);
				CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, initialUrl);
			}
			catch (Exception ex)
			{
				Logger.ErrorException(ex, "Something when wrong while creating the CEF client!");
				throw new Exception();
			}
		}

		/// <summary>
		///		Starts a loop that deals with the incoming events
		/// </summary>
		public void HandelEventsLoop()
		{
			//Setup ZMQ
			using ZContext context = new ZContext();
			using ZSocket responder = new ZSocket(context, ZSocketType.REP);

			responder.Bind($"tcp://127.0.0.1:{ipcPort}");

			isRunning = true;
			int eventPassingErrorCount = 0;
			while (isRunning)
			{
				//Get the json that was sent to us
				using ZFrame request = responder.ReceiveFrame();
				string json = request.ReadString();

				//Parse the data we get, and process it
				Logger.Debug(json);
				try
				{
					IEventData data = EventDataParser.ReadData(json);
					if (data == null)
						continue;

					if (data.EventType == EventType.Shutdown)
					{
						Logger.Debug("Got shutdown message...");
						isRunning = false;
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
					else if(data.EventType == EventType.ExecuteJsEvent)
						cefClient.ExecuteJs((data as ExecuteJsEvent)?.Js);

					responder.Send(new ZFrame((int) EventType.Ping));
				}
				catch (Exception ex)
				{
					eventPassingErrorCount++;
					Logger.ErrorException(ex, $"An error occurred while processing event data! Times left: {EventPassingNumErrorsAllowed - eventPassingErrorCount}");

					if (eventPassingErrorCount < EventPassingNumErrorsAllowed) continue;

					isRunning = false;
					break;
				}
			}
		}

		#region Destroy

		~CefBrowserProcess()
		{
			ReleaseResources();
		}
		
		public void Dispose()
		{
			ReleaseResources();
			GC.SuppressFinalize(this);
		}

		private void ReleaseResources()
		{
			isRunning = false;
			cefClient?.Dispose();
			CefRuntime.Shutdown();
		}

		#endregion
	}
}