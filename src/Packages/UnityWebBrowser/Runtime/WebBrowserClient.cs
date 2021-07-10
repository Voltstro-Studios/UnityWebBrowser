using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ServiceWire;
using ServiceWire.TcpIp;
using UnityEngine;
using UnityEngine.Serialization;
using UnityWebBrowser.BrowserEngine;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events.EngineAction;
using Debug = UnityEngine.Debug;
using MouseMoveEvent = UnityWebBrowser.Shared.Events.EngineAction.MouseMoveEvent;

namespace UnityWebBrowser
{
	/// <summary>
	///     Handles managing the process and worker thread
	/// </summary>
	[Serializable]
    public class WebBrowserClient : IDisposable
    {
        private const string LoggingTag = "[Web Browser]";

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

        private EngineProxy engineIpc;

        /// <summary>
        ///     Texture that the browser will paint to
        /// </summary>
        public Texture2D BrowserTexture { get; private set; }

        /// <summary>
        ///     <see cref="ILogger" /> that we log to
        /// </summary>
        internal ILogger Logger { get; private set; } = Debug.unityLogger;

        /// <summary>
        ///     Is the web browser client running?
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     The path that CEF will log to
        /// </summary>
        /// <exception cref="WebBrowserRunningException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public FileInfo LogPath
        {
            get => logPath;
            set
            {
                if (IsRunning)
                    throw new WebBrowserRunningException();

                logPath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        ///     The path to the cache
        /// </summary>
        /// <exception cref="WebBrowserRunningException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public FileInfo CachePath
        {
            get => cachePath;
            set
            {
                if (IsRunning)
                    throw new WebBrowserRunningException();

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

            string browserEngineMainDir = WebBrowserUtils.GetCefMainDirectory();

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

            //Cache, if cache is disabled Chromium will go into "incognito" mode
            if (cache)
            {
                cachePath ??= new FileInfo($"{browserEngineMainDir}/CEFCache");
                argsBuilder.AppendArgument("cache-path", cachePath.FullName, true);
            }
            
            //Add all our arguments that go directly to CEF last
            
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

            //Final built arguments
            string arguments = argsBuilder.ToString();

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
            serverProcess.OutputDataReceived += HandleCefProcessLog;
            serverProcess.Start();
            serverProcess.BeginOutputReadLine();
            serverProcess.BeginErrorReadLine();

            BrowserTexture = new Texture2D((int) width, (int) height, TextureFormat.BGRA32, false, false);

            Thread.Sleep(2000);
            
            IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, outPort);
            engineIpc = new EngineProxy(ip, new DefaultSerializer());
        }

        /// <summary>
        ///     Starts updating the <see cref="BrowserTexture" /> data
        /// </summary>
        /// <returns></returns>
        internal IEnumerator Start()
        {
            LogDebug("Starting communications between CEF process and Unity...");
            IsRunning = true;

            while (IsRunning)
            {
                yield return new WaitForSecondsRealtime(eventPollingTime);

                pixels = engineIpc.GetPixels();
            }
        }

        public void LoadTextureData()
        {
            byte[] pixelData = pixels;

            if (pixelData == null || pixelData.Length == 0)
                return;

            BrowserTexture.LoadRawTextureData(pixelData);
            BrowserTexture.Apply(false);
        }

        #region Pixels
        
        private byte[] pixels;

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

        private void HandleCefProcessLog(object sender, DataReceivedEventArgs e)
        {
            if (serverProcess.HasExited) return;

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

        #region Browser Controls

        /// <summary>
        ///     Sends a keyboard event
        /// </summary>
        /// <param name="keysDown"></param>
        /// <param name="keysUp"></param>
        /// <param name="chars"></param>
        public void SendKeyboardControls(int[] keysDown, int[] keysUp, string chars)
        {
            engineIpc.SendKeyboardEvent(new KeyboardEvent
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
        public void SendMouseMove(Vector2 mousePos)
        {
            engineIpc.SendMouseMoveEvent(new MouseMoveEvent
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
        public void SendMouseClick(Vector2 mousePos, int clickCount, MouseClickType clickType,
            MouseEventType eventType)
        {
            engineIpc.SendMouseClickEvent(new MouseClickEvent
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
        public void SendMouseScroll(int mouseX, int mouseY, int mouseScroll)
        {
            engineIpc.SendMouseScrollEvent(new MouseScrollEvent
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
        public void LoadUrl(string url)
        {
            engineIpc.LoadUrl(url);
        }

        /// <summary>
        ///     Tells the browser to go forward
        /// </summary>
        public void GoForward()
        {
            engineIpc.GoForward();
        }

        /// <summary>
        ///     Tells the browser to go back
        /// </summary>
        public void GoBack()
        {
            engineIpc.GoBack();
        }

        /// <summary>
        ///     Refreshes the browser
        /// </summary>
        public void Refresh()
        {
            engineIpc.Refresh();
        }

        /// <summary>
        ///     Makes the browser load html
        /// </summary>
        /// <param name="html"></param>
        public void LoadHtml(string html)
        {
            engineIpc.LoadHtml(html);
        }

        /// <summary>
        ///     Executes JS in the browser
        /// </summary>
        /// <param name="js"></param>
        public void ExecuteJs(string js)
        {
            engineIpc.ExecuteJs(js);
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
            if (!IsRunning)
                return;
            
            engineIpc.Shutdown();
            engineIpc.Dispose();

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