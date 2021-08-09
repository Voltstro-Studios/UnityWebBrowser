using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityWebBrowser.BrowserEngine;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;
using VoltRpc.Communication;
using Debug = UnityEngine.Debug;

namespace UnityWebBrowser
{
	/// <summary>
	///     Handles managing the browser engine process
	/// </summary>
	[Serializable]
    public class WebBrowserClient : IDisposable
    {
        private const string LoggingTag = "[Web Browser]";
        private const string ActiveEngineFileName = "EngineActive";

        /// <summary>
        ///     The active browser engine this instance is using
        /// </summary>
        [Header("Browser Settings")] 
        [Tooltip("The active browser engine this instance is using")]
        [ActiveBrowserEngine] public string browserEngine;
        
        /// <summary>
        ///     The initial URl the browser will start at
        /// </summary>
        [Tooltip("The initial URl the browser will start at")]
        public string initialUrl = "https://voltstro.dev";

        /// <summary>
        ///     The width of the browser
        /// </summary>
        [Tooltip("The width of the browser")] public uint width = 1920;

        /// <summary>
        ///     The height of the browser
        /// </summary>
        [Tooltip("The height of the browser")] public uint height = 1080;

        /// <summary>
        ///     The background <see cref="Color32" /> of the webpage
        /// </summary>
        [Tooltip("The background color of the webpage")]
        public Color32 backgroundColor = new Color32(255, 255, 255, 255);

        /// <summary>
        ///     Enable or disable JavaScript
        /// </summary>
        [Tooltip("Enable or disable JavaScript")]
        public bool javascript = true;

        /// <summary>
        ///     Enable or disable the cache
        /// </summary>
        [Tooltip("Enable or disable the cache")]
        public bool cache = true;

        /// <summary>
        ///     Enable or disable WebRTC
        /// </summary>
        [Tooltip("Enable or disable WebRTC")]
        public bool webRtc;

        /// <summary>
        ///     Proxy Settings
        /// </summary>
        [Tooltip("Proxy settings")] 
        public ProxySettings proxySettings;

        /// <summary>
        ///     Enable or disable remote debugging
        /// </summary>
        [Tooltip("Enable or disable remote debugging")]
        public bool remoteDebugging;

        /// <summary>
        ///     The port to use for remote debugging
        /// </summary>
        [Tooltip("The port to use for remote debugging")]
        public int remoteDebuggingPort = 9022;

        /// <summary>
        ///     The port to communicate with the browser process on
        /// </summary>
        [Header("IPC Settings")]
        [FormerlySerializedAs("port")]
        [Tooltip("The port to communicate with the browser process on")]
        public int outPort = 5555;

        /// <summary>
        ///     The port to communicate with the browser process on
        /// </summary>
        [Tooltip("The port to communicate with the browser process on")]
        public int inPort = 5556;

        /// <summary>
        ///     Timeout time for connection (in milliseconds)
        /// </summary>
        [Tooltip("Timeout time for connection (in milliseconds)")]
        public int connectionTimeout = 100000;

        /// <summary>
        ///     Timeout time for waiting for the engine to start (in milliseconds)
        /// </summary>
        [Tooltip("Timeout time for waiting for the engine to start (in milliseconds)")]
        public int engineStartupTimeout = 100000;
        
        /// <summary>
        ///     The time between each frame sent the browser process
        /// </summary>
        [Tooltip("The time between each frame sent the browser process")]
        public float eventPollingTime = 0.04f;

        /// <summary>
        ///     The log severity. Only messages of this severity level or higher will be logged
        /// </summary>
        [Tooltip("The log severity. Only messages of this severity level or higher will be logged")]
        public LogSeverity logSeverity;

        private FileInfo cachePath;

        private FileInfo logPath;

        private Process serverProcess;

        private WebBrowserCommunicationsManager communicationsManager;

        /// <summary>
        ///     Texture that the browser will paint to
        /// </summary>
        public Texture2D BrowserTexture { get; private set; }

        /// <summary>
        ///     <see cref="ILogger" /> that we log to
        /// </summary>
        internal ILogger Logger { get; private set; } = Debug.unityLogger;

        /// <summary>
        ///     Is the Unity client connected to the browser engine
        /// </summary>
        public bool IsConnected => communicationsManager is { IsConnected: true };

        /// <summary>
        ///     The path that CEF will log to
        /// </summary>
        /// <exception cref="WebBrowserIsConnectedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public FileInfo LogPath
        {
            get => logPath;
            set
            {
                if (IsConnected)
                    throw new WebBrowserIsConnectedException("You cannot change the log path once the browser engine is connected");

                logPath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        ///     The path to the cache
        /// </summary>
        /// <exception cref="WebBrowserIsConnectedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public FileInfo CachePath
        {
            get => cachePath;
            set
            {
                if (IsConnected)
                    throw new WebBrowserIsConnectedException("You cannot change the cache path once the browser engine is connected");

                if (!cache)
                    throw new ArgumentException("The cache is disabled!");

                cachePath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        ///     Inits the browser client
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        internal void Init()
        {
            //Get the path to the CEF browser process and make sure it exists
            string browserEnginePath = WebBrowserUtils.GetBrowserEngineProcessPath(browserEngine);
            LogDebug($"Starting browser engine process from {browserEnginePath}");

            if (!File.Exists(browserEnginePath))
            {
                LogError("The browser engine process doesn't exist!");
                throw new FileNotFoundException($"{browserEngine} process could not be found!");
            }

            string browserEngineMainDir = WebBrowserUtils.GetBrowserEngineMainDirectory();

            //Start to build our arguments
            WebBrowserArgsBuilder argsBuilder = new WebBrowserArgsBuilder();
            
            //Initial URL
            argsBuilder.AppendArgument("initial-url", initialUrl, true);
            
            //Width & Height
            argsBuilder.AppendArgument("width", width);
            argsBuilder.AppendArgument("height", height);
            
            //Javascript
            argsBuilder.AppendArgument("javascript", javascript);
            
            //Background color
            argsBuilder.AppendArgument("bcr", backgroundColor.r);
            argsBuilder.AppendArgument("bcg", backgroundColor.g);
            argsBuilder.AppendArgument("bcb", backgroundColor.b);
            argsBuilder.AppendArgument("bca", backgroundColor.a);

            //Logging
            LogPath ??= new FileInfo($"{browserEngineMainDir}/{browserEngine}.log");
            argsBuilder.AppendArgument("log-path", LogPath.FullName, true);
            argsBuilder.AppendArgument("log-severity", logSeverity);

            //IPC ports
            argsBuilder.AppendArgument("in-port", outPort);
            argsBuilder.AppendArgument("out-port", inPort);

            //If we have a cache, set the cache path
            if (cache)
            {
                cachePath ??= new FileInfo($"{browserEngineMainDir}/BrowserCache");
                argsBuilder.AppendArgument("cache-path", cachePath.FullName, true);
            }

            //Setup web RTC
            if(webRtc)
                argsBuilder.AppendArgument("web-rtc", webRtc);
            
            //Setup remote debugging
            if(remoteDebugging)
                argsBuilder.AppendArgument("remote-debugging", remoteDebuggingPort);
            
            //Setup proxy
            argsBuilder.AppendArgument("proxy-server", proxySettings.ProxyServer);
            if(!string.IsNullOrWhiteSpace(proxySettings.Username))
                argsBuilder.AppendArgument("proxy-username", proxySettings.Username, true);
                
            if(!string.IsNullOrWhiteSpace(proxySettings.Password))
                argsBuilder.AppendArgument("proxy-password", proxySettings.Password, true);
            
            //Our engine active file
            argsBuilder.AppendArgument("active-engine-file-path", browserEngineMainDir, true);

            //Final built arguments
            string arguments = argsBuilder.ToString();

            //Setup communication manager
            communicationsManager = new WebBrowserCommunicationsManager(connectionTimeout, outPort, inPort);
            communicationsManager.Listen();
            communicationsManager.OnUrlChanged += url => OnUrlChanged?.Invoke(url);
            communicationsManager.OnLoadStart += url => OnLoadStart?.Invoke(url);
            communicationsManager.OnLoadFinish += url => OnLoadFinish?.Invoke(url);

            //Start the server process
            serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo(browserEnginePath, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = WebBrowserUtils.GetBrowserEnginePath(browserEngine)
                },
                EnableRaisingEvents = true
            };
            serverProcess.OutputDataReceived += HandleEngineProcessLog;
            serverProcess.Start();
            serverProcess.BeginOutputReadLine();
            serverProcess.BeginErrorReadLine();

            try
            {
                WebBrowserUtils.WaitForActiveEngineFile(
                    Path.GetFullPath($"{browserEngineMainDir}/{ActiveEngineFileName}"), engineStartupTimeout);
            }
            catch (TimeoutException)
            {
                LogError("The engine failed to startup in time!");
                if(!serverProcess.HasExited)
                    serverProcess.KillTree();
                throw;
            }

            BrowserTexture = new Texture2D((int) width, (int) height, TextureFormat.BGRA32, false, false);

            try
            {
                communicationsManager.Connect();
            }
            catch (Exception)
            {
                serverProcess.KillTree();
                LogError("An error occured while connecting!");
                throw;
            }
        }

        #region Main Loop

        private byte[] pixels;
        
        /// <summary>
        ///     Starts updating the <see cref="BrowserTexture" /> data
        /// </summary>
        /// <returns></returns>
        internal IEnumerator Start()
        {
            LogDebug("Starting communications between engine process and Unity...");

            while (IsConnected)
            {
                yield return new WaitForSecondsRealtime(eventPollingTime);

                pixels = communicationsManager.GetPixels();
            }
        }

        /// <summary>
        ///     Loads the pixel data into the <see cref="BrowserTexture"/>
        /// </summary>
        public void LoadTextureData()
        {
            byte[] pixelData = pixels;

            if (pixelData == null || pixelData.Length == 0)
                return;

            BrowserTexture.LoadRawTextureData(pixelData);
            BrowserTexture.Apply(false);
        }
        
        #endregion

        #region Logging

        /// <summary>
        ///     Logs a debug message
        /// </summary>
        /// <param name="message"></param>
        internal void LogDebug(object message)
        {
            Logger.Log(LogType.Log, LoggingTag, message);
        }

        /// <summary>
        ///     Logs a warning
        /// </summary>
        /// <param name="message"></param>
        internal void LogWarning(object message)
        {
            Logger.LogWarning(LoggingTag, message);
        }

        /// <summary>
        ///     Logs a error
        /// </summary>
        /// <param name="message"></param>
        internal void LogError(object message)
        {
            Logger.LogError(LoggingTag, message);
        }

        /// <summary>
        ///     Replaces the logger the web browser will use
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ReplaceLogger(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void HandleEngineProcessLog(object sender, DataReceivedEventArgs e)
        {
            if (serverProcess.HasExited) 
                return;

            if (e.Data.StartsWith("DEBUG "))
                LogDebug(e.Data.Replace("DEBUG ", ""));
            else if (e.Data.StartsWith("INFO "))
                LogDebug(e.Data.Replace("INFO ", ""));
            else if (e.Data.StartsWith("WARN "))
                LogWarning(e.Data.Replace("WARN ", ""));
            else if (e.Data.StartsWith("ERROR "))
                LogError(e.Data.Replace("ERROR ", ""));
            else
                LogDebug(e.Data);
        }

        #endregion

        #region Browser Events

        /// <summary>
        ///     Invoked when the url changes
        /// </summary>
        public event Action<string> OnUrlChanged;

        /// <summary>
        ///     Invoked when the page starts to load
        /// </summary>
        public event Action<string> OnLoadStart;

        /// <summary>
        ///     Invoked when the page finishes loading
        /// </summary>
        public event Action<string> OnLoadFinish; 

        #endregion

        #region Browser Controls

        /// <summary>
        ///     Sends a keyboard event
        /// </summary>
        /// <param name="keysDown"></param>
        /// <param name="keysUp"></param>
        /// <param name="chars"></param>
        internal void SendKeyboardControls(int[] keysDown, int[] keysUp, string chars)
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.SendKeyboardEvent(new KeyboardEvent
            {
                KeysDown = keysDown,
                KeysUp = keysUp,
                Chars = chars
            });
        }

        /// <summary>
        ///     Sends a mouse event
        /// </summary>
        /// <param name="mousePos"></param>
        internal void SendMouseMove(Vector2 mousePos)
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.SendMouseMoveEvent(new MouseMoveEvent
            {
                MouseX = (int) mousePos.x,
                MouseY = (int) mousePos.y
            });
        }

        /// <summary>
        ///     Sends a mouse click event
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="clickCount"></param>
        /// <param name="clickType"></param>
        /// <param name="eventType"></param>
        internal void SendMouseClick(Vector2 mousePos, int clickCount, MouseClickType clickType,
            MouseEventType eventType)
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.SendMouseClickEvent(new MouseClickEvent
            {
                MouseX = (int) mousePos.x,
                MouseY = (int) mousePos.y,
                MouseClickCount = clickCount,
                MouseClickType = clickType,
                MouseEventType = eventType
            });
        }

        /// <summary>
        ///     Sends a mouse scroll event
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <param name="mouseScroll"></param>
        internal void SendMouseScroll(int mouseX, int mouseY, int mouseScroll)
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.SendMouseScrollEvent(new MouseScrollEvent
            {
                MouseScroll = mouseScroll,
                MouseX = mouseX,
                MouseY = mouseY
            });
        }

        /// <summary>
        ///     Tells the browser to load a URL
        /// </summary>
        /// <param name="url"></param>
        internal void LoadUrl(string url)
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.LoadUrl(url);
        }

        /// <summary>
        ///     Tells the browser to go forward
        /// </summary>
        internal void GoForward()
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.GoForward();
        }

        /// <summary>
        ///     Tells the browser to go back
        /// </summary>
        internal void GoBack()
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.GoBack();
        }

        /// <summary>
        ///     Refreshes the browser
        /// </summary>
        internal void Refresh()
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.Refresh();
        }

        /// <summary>
        ///     Makes the browser load html
        /// </summary>
        /// <param name="html"></param>
        internal void LoadHtml(string html)
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.LoadHtml(html);
        }

        /// <summary>
        ///     Executes JS in the browser
        /// </summary>
        /// <param name="js"></param>
        internal void ExecuteJs(string js)
        {
            if (!IsConnected)
                throw new NotConnectedException("The Unity client is not connected to the browser engine!");
            
            communicationsManager.ExecuteJs(js);
        }

        #endregion

        #region Destroying

        ~WebBrowserClient()
        {
            ReleaseResources();
        }

        /// <summary>
        ///     Destroys this <see cref="WebBrowserClient" /> instance
        /// </summary>
        public void Dispose()
        {
            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseResources()
        {
            if (!IsConnected)
                return;

            try
            {
                communicationsManager.Dispose();
            }
            catch (NotConnectedException) //Force kill if we are not connected
            {
                serverProcess.KillTree();
                return;
            }

            WaitForServerProcess().ConfigureAwait(false);

            LogDebug("Web browser shutdown.");
        }

        private async Task WaitForServerProcess()
        {
            await Task.Delay(5000);

            if (!serverProcess.HasExited)
            {
                serverProcess.KillTree();
                LogWarning("Forced killed web browser process!");
            }
            
            serverProcess.Dispose();
        }

        #endregion
    }
}