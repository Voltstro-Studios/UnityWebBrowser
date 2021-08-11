using System;
using System.Linq;
using UnityWebBrowser.Engine.Cef.Browser;
using UnityWebBrowser.Engine.Shared;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core
{
	/// <summary>
	///     Manager for CEF
	/// </summary>
	internal class CefEngineManager : IEngine, IDisposable
    {
        private readonly string[] args;
        private readonly LaunchArguments launchArguments;

        private UwbCefClient cefClient;

        /// <summary>
        ///     Creates a new <see cref="CefEngineManager" /> instance
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="rawArguments"></param>
        /// <exception cref="DllNotFoundException"></exception>
        /// <exception cref="CefVersionMismatchException"></exception>
        /// <exception cref="Exception"></exception>
        public CefEngineManager(LaunchArguments arguments, string[] rawArguments)
        {
            //Setup CEF
            CefRuntime.Load();

            launchArguments = arguments;
            args = rawArguments;
        }

        /// <summary>
        ///     Starts CEF
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
            CefMainArgs cefMainArgs = new(argv);
            UwbCefApp cefApp = new(launchArguments);

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
            CefSettings cefSettings = new()
            {
                WindowlessRenderingEnabled = true,
                NoSandbox = true,
                LogFile = launchArguments.LogPath.FullName,
                CachePath = cachePathArgument,
                MultiThreadedMessageLoop = false,
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
            CefColor backgroundColor = new(launchArguments.Bca, launchArguments.Bcr, launchArguments.Bcg,
                launchArguments.Bcb);
            CefBrowserSettings cefBrowserSettings = new()
            {
                BackgroundColor = backgroundColor,
                JavaScript = launchArguments.JavaScript ? CefState.Enabled : CefState.Disabled,
                LocalStorage = CefState.Disabled
            };

            Logger.Debug("CEF starting with these options:" +
                         $"\nJS: {launchArguments.JavaScript}" +
                         $"\nBackgroundColor: {backgroundColor}" +
                         $"\nCache Path: {cachePathArgument}" +
                         $"\nLog Path: {launchArguments.LogPath.FullName}" +
                         $"\nLog Severity: {launchArguments.LogSeverity}");
            Logger.Info("Starting CEF client...");

            //Create cef browser
            cefClient = new UwbCefClient(new CefSize(launchArguments.Width, launchArguments.Height),
                new ProxySettings(launchArguments.ProxyUsername, launchArguments.ProxyPassword,
                    launchArguments.ProxyEnabled));
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, launchArguments.InitialUrl);

            //Setup cef events
            cefClient.OnUrlChange += url => OnUrlChanged?.Invoke(url);
            cefClient.OnLoadStart += url => OnLoadStart?.Invoke(url);
            cefClient.OnLoadFinish += url => OnLoadFinish?.Invoke(url);
        }

        public event Action<string> OnUrlChanged;
        public event Action<string> OnLoadStart;
        public event Action<string> OnLoadFinish;

        private static void PostTask(CefThreadId threadId, Action action)
        {
            CefRuntime.PostTask(threadId, new CefActionTask(action));
        }

        #region Engine Actions

        public byte[] GetPixels()
        {
            return cefClient.GetPixels();
        }

        public void Shutdown()
        {
            //We can only quit the message loop on the UI (main) thread
            if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
            {
                PostTask(CefThreadId.UI, Shutdown);
                return;
            }

            Logger.Debug("Quitting message loop...");
            CefRuntime.QuitMessageLoop();
        }

        public void SendKeyboardEvent(KeyboardEvent keyboardEvent)
        {
            cefClient.ProcessKeyboardEvent(keyboardEvent);
        }

        public void SendMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
        {
            cefClient.ProcessMouseMoveEvent(mouseMoveEvent);
        }

        public void SendMouseClickEvent(MouseClickEvent mouseClickEvent)
        {
            cefClient.ProcessMouseClickEvent(mouseClickEvent);
        }

        public void SendMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
        {
            cefClient.ProcessMouseScrollEvent(mouseScrollEvent);
        }

        public void GoForward()
        {
            cefClient.GoForward();
        }

        public void GoBack()
        {
            cefClient.GoBack();
        }

        public void Refresh()
        {
            cefClient.Refresh();
        }

        public void LoadUrl(string url)
        {
            cefClient.LoadUrl(url);
        }

        public void LoadHtml(string html)
        {
            cefClient.LoadHtml(html);
        }

        public void ExecuteJs(string js)
        {
            cefClient.ExecuteJs(js);
        }

        #endregion

        #region Destroy

        ~CefEngineManager()
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