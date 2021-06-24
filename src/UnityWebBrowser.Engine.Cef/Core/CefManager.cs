using System;
using System.Linq;
using UnityWebBrowser.Engine.Cef.Browser;
using UnityWebBrowser.Engine.Shared;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core
{
	/// <summary>
	///		Manager for CEF
	/// </summary>
    internal class CefManager : IDisposable
    {
	    private readonly LaunchArguments launchArguments;
	    private readonly string[] args;
	    
	    private BrowserProcessCEFClient cefClient;
	    
	    /// <summary>
	    ///		Creates a new <see cref="CefManager"/> instance
	    /// </summary>
	    /// <param name="arguments"></param>
	    /// <param name="rawArguments"></param>
	    /// <exception cref="DllNotFoundException"></exception>
	    /// <exception cref="CefVersionMismatchException"></exception>
	    /// <exception cref="Exception"></exception>
	    public CefManager(LaunchArguments arguments, string[] rawArguments)
        {
            //Setup CEF
            CefRuntime.Load();

            launchArguments = arguments;
            args = rawArguments;
        }

	    /// <summary>
	    ///		Starts CEF
	    /// </summary>
	    /// <exception cref="Exception"></exception>
	    public void Init()
	    {
		    // ReSharper disable once RedundantAssignment
			string[] argv = args;
#if LINUX
	        //On Linux we need to do this, otherwise it will just crash, no idea why tho
			argv = new string[args.Length + 1];
			Array.Copy(args, 0, argv, 1, args.Length);
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
			
			//Do we have a cache or not, if not CEF will run in "incognito" mode.
			string cachePathArgument = null;
			if (launchArguments.CachePath != null)
				cachePathArgument = launchArguments.CachePath.FullName;
			
			//Convert UnityWebBrowser log severity to CefLogSeverity
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
				
				//TODO: On MacOS multi-threaded message loop isn't supported
				MultiThreadedMessageLoop = true,
				LogSeverity = logSeverity,
				Locale = "en-US",
				ExternalMessagePump = false,
				RemoteDebuggingPort = launchArguments.RemoteDebugging,
#if LINUX
				//On Linux we need to tell CEF where everything is, this will assume that the working directory is where everything is!
				ResourcesDirPath = System.IO.Path.Combine(Environment.CurrentDirectory),
				LocalesDirPath = System.IO.Path.Combine(Environment.CurrentDirectory, "locales"),
				BrowserSubprocessPath = Environment.GetCommandLineArgs()[0]
#endif
			};

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
			cefClient = new BrowserProcessCEFClient(new CefSize(launchArguments.Width, launchArguments.Height), 
					new ProxySettings(launchArguments.ProxyUsername, launchArguments.ProxyPassword, launchArguments.ProxyEnabled));
		    CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, launchArguments.InitialUrl);
	    }

	    /// <summary>
	    ///		Gets the browser's pixels
	    /// </summary>
	    /// <returns></returns>
	    public byte[] GetPixels()
	    {
		    return cefClient.GetPixels();
	    }

	    /// <summary>
	    ///		Handle a <see cref="KeyboardEvent"/>
	    /// </summary>
	    /// <param name="keyboardEvent"></param>
	    public void HandelKeyboardEvent(KeyboardEvent keyboardEvent)
	    {
		    cefClient.ProcessKeyboardEvent(keyboardEvent);
	    }

	    /// <summary>
	    ///		Handel a <see cref="MouseMoveEvent"/>
	    /// </summary>
	    /// <param name="mouseMoveEvent"></param>
	    public void HandelMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
	    {
		    cefClient.ProcessMouseMoveEvent(mouseMoveEvent);
	    }

	    /// <summary>
	    ///		Handel a <see cref="MouseClickEvent"/>
	    /// </summary>
	    /// <param name="mouseClickEvent"></param>
	    public void HandelMouseClickEvent(MouseClickEvent mouseClickEvent)
	    {
		    cefClient.ProcessMouseClickEvent(mouseClickEvent);
	    }

	    /// <summary>
	    ///		Handel a <see cref="MouseScrollEvent"/>
	    /// </summary>
	    /// <param name="mouseScrollEvent"></param>
	    public void HandelMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
	    {
		    cefClient.ProcessMouseScrollEvent(mouseScrollEvent);
	    }

	    /// <summary>
	    ///		Makes CEF go forward
	    /// </summary>
	    public void GoForward()
	    {
		    cefClient.GoForward();
	    }

	    /// <summary>
	    ///		Makes CEF go back
	    /// </summary>
	    public void GoBack()
	    {
		    cefClient.GoBack();
	    }

	    /// <summary>
	    ///		Makes CEF reload the page
	    /// </summary>
	    public void Refresh()
	    {
		    cefClient.Refresh();
	    }

	    /// <summary>
	    ///		Makes CEF load some HTML
	    /// </summary>
	    /// <param name="html"></param>
	    public void LoadHtml(string html)
	    {
		    cefClient.LoadHtml(html);
	    }

	    /// <summary>
	    ///		Makes CEF load a URL
	    /// </summary>
	    /// <param name="url"></param>
	    public void LoadUrl(string url)
	    {
		    cefClient.LoadUrl(url);
	    }

	    /// <summary>
	    ///		Makes CEF execute some JS on the current page
	    /// </summary>
	    /// <param name="js"></param>
	    public void ExecuteJs(string js)
	    {
		    cefClient.ExecuteJs(js);
	    }

	    #region Destroy

	    ~CefManager()
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
		    cefClient?.Dispose();
		    CefRuntime.Shutdown();
	    }
	    
	    #endregion
    }
}