using System;
using System.Linq;
using UnityWebBrowser.Engine.Cef.Browser;
using UnityWebBrowser.Engine.Cef.Models;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineActions;
using UnityWebBrowser.Shared.Events.EngineEvents;
using Xilium.CefGlue;
using ZeroMQ;

namespace UnityWebBrowser.Engine.Cef.Core
{
	/// <summary>
	///		Main class responsible for the app
	///		<para>
	///			This class handles managing CEF and talks back to the client using ZMQ
	///		</para>
	/// </summary>
	public class CefBrowserProcess : IDisposable
	{
		private readonly EventReplier<EngineActionEvent, EngineEvent> eventReplier;
		private readonly BrowserProcessCEFClient cefClient;

		///  <summary>
		/// 		Creates a new <see cref="CefBrowserProcess"/> instance
		///  </summary>
		///  <param name="launchArguments"></param>
		///  <param name="cefArgs"></param>
		///  <exception cref="Exception"></exception>
		public CefBrowserProcess(LaunchArguments launchArguments, string[] cefArgs)
		{
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
			if (launchArguments.CachePath != null)
				cachePathArgument = launchArguments.CachePath.FullName;
			
			//Log severity
			CefLogSeverity logSeverity = launchArguments.LogSeverity switch
			{
				LogSeverity.Debug => CefLogSeverity.Debug,
				LogSeverity.Info => CefLogSeverity.Info,
				LogSeverity.Warn => CefLogSeverity.Warning,
				LogSeverity.Error => CefLogSeverity.Error,
				LogSeverity.Fatal => CefLogSeverity.Fatal,
				_ => CefLogSeverity.Default
			};

			//Setup the CEF settings
			CefSettings cefSettings = new CefSettings
			{
				WindowlessRenderingEnabled = true,
				NoSandbox = true,
				LogFile = launchArguments.LogPath.FullName,
				CachePath = cachePathArgument,
				MultiThreadedMessageLoop = true,
				LogSeverity = logSeverity,
				Locale = "en-US",
				ExternalMessagePump = false,
				RemoteDebuggingPort = launchArguments.RemoteDebugging,
#if LINUX
				//On Linux we need to tell CEF where everything is
				ResourcesDirPath = System.IO.Path.Combine(Environment.CurrentDirectory),
				LocalesDirPath = System.IO.Path.Combine(Environment.CurrentDirectory, "locales"),
				BrowserSubprocessPath = Environment.GetCommandLineArgs()[0]
#endif
			};
			
			// ReSharper disable once RedundantAssignment
			string[] argv = cefArgs;
#if LINUX
			argv = new string[cefArgs.Length + 1];
			Array.Copy(cefArgs, 0, argv, 1, cefArgs.Length);
			argv[0] = "-";
#endif

			//Set up CEF args and the CEF app
			CefMainArgs cefMainArgs = new CefMainArgs(argv);
			BrowserProcessCEFApp cefApp = new BrowserProcessCEFApp(launchArguments);
			
			//Run our sub-processes
			int exitCode = CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero);
			if (exitCode != -1)
			{
				Environment.Exit(exitCode);
				return;
			}
			
			//Backup
			if (argv.Any(arg => arg.StartsWith("--type=")))
			{
				Environment.Exit(-2);
				return;
			}

			//Init CEF
			CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

			//Create a CEF window and set it to windowless
			CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
			cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

			//Create our CEF browser settings
			CefColor backgroundColor = new CefColor(launchArguments.Bca, launchArguments.Bcr, launchArguments.Bcg,
				launchArguments.Bcb);
			CefBrowserSettings cefBrowserSettings = new CefBrowserSettings
			{
				BackgroundColor = backgroundColor,
				JavaScript = launchArguments.JavaScript ? CefState.Enabled : CefState.Disabled,
				LocalStorage = CefState.Disabled
			};

			Logger.Debug($"CEF starting with these options:" +
			             $"\nJS: {launchArguments.JavaScript}" +
			             $"\nBackgroundColor: {backgroundColor}" +
			             $"\nCache Path: {cachePathArgument}" +
			             $"\nLog Path: {launchArguments.LogPath.FullName}" +
			             $"\nLog Severity: {launchArguments.LogSeverity}");
			Logger.Info("Starting CEF client...");

			//Create cef browser
			try
			{
				cefClient = new BrowserProcessCEFClient(new CefSize(launchArguments.Width, launchArguments.Height), 
					new ProxySettings(launchArguments.ProxyUsername, launchArguments.ProxyPassword));
				CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, launchArguments.InitialUrl);
			}
			catch (Exception ex)
			{
				Logger.ErrorException(ex, "Something when wrong while creating the CEF client!");
				throw new Exception();
			}

			eventReplier = new EventReplier<EngineActionEvent, EngineEvent>(launchArguments.Port, OnEventReceived);
		}

		private EngineEvent OnEventReceived(EngineActionEvent actionEvent)
		{
			switch (actionEvent)
			{
				case ShutdownEvent:
					Dispose();
					break;
				case PingEvent:
					return new PixelsEvent
					{
						Pixels = cefClient.GetPixels()
					};
				case KeyboardEvent x:
					cefClient.ProcessKeyboardEvent(x);
					break;
				case GoForwardEvent:
					cefClient.GoForward();
					break;
				case GoBackEvent:
					cefClient.GoBack();
					break;
				case RefreshEvent:
					cefClient.Refresh();
					break;
				case NavigateUrlEvent x:
					cefClient.LoadUrl(x.Url);
					break;
				case MouseMoveEvent x:
					cefClient.ProcessMouseMoveEvent(x);
					break;
				case MouseClickEvent x:
					cefClient.ProcessMouseClickEvent(x);
					break;
				case MouseScrollEvent x:
					cefClient.ProcessMouseScrollEvent(x);
					break;
				case LoadHtmlEvent x:
					cefClient.LoadHtml(x.Html);
					break;
				case ExecuteJsEvent x:
					cefClient.ExecuteJs(x.Js);
					break;
			}

			return new OkEvent();
		}

		/// <summary>
		///		Starts a loop that deals with the incoming events
		/// </summary>
		public void HandelEventsLoop()
		{
			eventReplier.HandleEventsLoop();
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
			eventReplier.Dispose();
			cefClient?.Dispose();
			CefRuntime.Shutdown();
		}

		#endregion
	}
}