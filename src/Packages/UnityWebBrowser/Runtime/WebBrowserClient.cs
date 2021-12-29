using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityWebBrowser.BrowserEngine;
using UnityWebBrowser.Events;
using UnityWebBrowser.Logging;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events;
using Object = UnityEngine.Object;
using Resolution = UnityWebBrowser.Shared.Resolution;

namespace UnityWebBrowser
{
    /// <summary>
    ///     The main object responsible for UWB.
    ///     <para>
    ///         This class handles:
    ///         <list type="bullet">
    ///              <item>UWB process setup</item>
    ///              <item>Texture setup and rendering</item>
    ///              <item>Wrapper for invoking methods on the UWB process</item>
    ///              <item>Shutdown</item>
    ///          </list>
    ///          If you need to do something with UWB, its probably here.
    ///     </para>
    /// </summary>
    [Serializable]
    public class WebBrowserClient : IDisposable
    {
        private const string ActiveEngineFileName = "EngineActive";

        private static ProfilerMarker browserPixelDataMarker = new ProfilerMarker("UWB.LoadPixelData");
        private static ProfilerMarker browserLoadTextureMarker = new ProfilerMarker("UWB.LoadTextureData");

        /// <summary>
        ///     The active browser engine this instance is using
        /// </summary>
        [Header("Browser Settings")] [Tooltip("The active browser engine this instance is using")] [ActiveBrowserEngine]
        public string browserEngine;

        /// <summary>
        ///     The initial URl the browser will start at
        /// </summary>
        [Tooltip("The initial URl the browser will start at")]
        public string initialUrl = "https://voltstro.dev";

        #region Resoltuion

        [SerializeField] private Resolution resolution = new(1920, 1080);

        /// <summary>
        ///     The resolution of the browser
        /// </summary>
        public Resolution Resolution
        {
            get => resolution;
            set
            {
                resolution = value;
                Resize(value);
            }
        }

        #endregion

        /// <summary>
        ///     The background <see cref="Color32" /> of the webpage
        /// </summary>
        [Tooltip("The background color of the webpage")]
        public Color32 backgroundColor = new(255, 255, 255, 255);

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
        [Range(1024, 65353)]
        public uint remoteDebuggingPort = 9022;

        /// <summary>
        ///     Settings related to IPC
        /// </summary>
        [Header("IPC Settings")] 
        public WebBrowserIpcSettings ipcSettings = new();

        /// <summary>
        ///     Timeout time for waiting for the engine to start (in milliseconds)
        /// </summary>
        [Tooltip("Timeout time for waiting for the engine to start (in milliseconds)")]
        public int engineStartupTimeout = 100000;

        /// <summary>
        ///     The log severity. Only messages of this severity level or higher will be logged
        /// </summary>
        [Tooltip("The log severity. Only messages of this severity level or higher will be logged")]
        public LogSeverity logSeverity;

        /// <summary>
        ///     Texture that the browser will paint to
        /// </summary>
        public Texture2D BrowserTexture { get; private set; }

        /// <summary>
        ///     Are we connected to the UW engine process
        /// </summary>
        public bool IsConnected => communicationsManager is { IsConnected: true };

        #region IsReady

        private object isReadyLock;
        private bool isReady;
        
        /// <summary>
        ///     Is UWB ready?
        /// </summary>
        public bool IsReady 
        {
            get
            {
                lock (isReadyLock)
                {
                    return isReady;
                }
            }
            private set
            {
                lock (isReadyLock)
                {
                    isReady = value;
                }
            }
        }

        #endregion

        #region Log Path

        private FileInfo logPath;

        /// <summary>
        ///     The path that UWB engine will log to
        /// </summary>
        /// <exception cref="WebBrowserIsConnectedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public FileInfo LogPath
        {
            get => logPath;
            set
            {
                if (IsConnected)
                    throw new WebBrowserIsConnectedException(
                        "You cannot change the log path once the browser engine is connected");

                logPath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        #region Cache Path

        private FileInfo cachePath;

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
                    throw new WebBrowserIsConnectedException(
                        "You cannot change the cache path once the browser engine is connected");

                if (!cache)
                    throw new ArgumentException("The cache is disabled!");

                cachePath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        #region Logger

        /// <summary>
        ///     Internal usage of <see cref="IWebBrowserLogger"/>
        /// </summary>
        internal IWebBrowserLogger logger = new DefaultUnityWebBrowserLogger();

        /// <summary>
        ///     Gets the <see cref="IWebBrowserLogger"/> to use for logging
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IWebBrowserLogger Logger
        {
            get => logger;
            set => logger = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion
        
        private Process serverProcess;
        private WebBrowserCommunicationsManager communicationsManager;

        /// <summary>
        ///     Inits the browser client
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        internal void Init()
        {
            //Get the path to the UWB process we are using and make sure it exists
            string browserEnginePath = WebBrowserUtils.GetBrowserEngineProcessPath(browserEngine);
            logger.Debug($"Starting browser engine process from '{browserEnginePath}'...");

            if (!File.Exists(browserEnginePath))
            {
                logger.Error("The browser engine process doesn't exist!");
                throw new FileNotFoundException($"{browserEngine} process could not be found!");
            }

            isReadyLock = new object();
            
            //Setup texture
            BrowserTexture = new Texture2D((int)resolution.Width, (int)resolution.Height, TextureFormat.BGRA32, false, false);
            WebBrowserUtils.SetAllTextureColorToOne(BrowserTexture, backgroundColor);

            string browserEngineMainDir = WebBrowserUtils.GetBrowserEngineMainDirectory();

            //Start to build our arguments
            WebBrowserArgsBuilder argsBuilder = new WebBrowserArgsBuilder();

            //Initial URL
            argsBuilder.AppendArgument("initial-url", initialUrl, true);

            //Width & Height
            argsBuilder.AppendArgument("width", resolution.Width);
            argsBuilder.AppendArgument("height", resolution.Height);

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

            //IPC settings
            argsBuilder.AppendArgument("pipes", ipcSettings.preferPipes);
            if (ipcSettings.preferPipes)
            {
                argsBuilder.AppendArgument("in-location", ipcSettings.outPipeName, true);
                argsBuilder.AppendArgument("out-location", ipcSettings.inPipeName, true);
            }
            else
            {
                argsBuilder.AppendArgument("in-location", ipcSettings.outPort);
                argsBuilder.AppendArgument("out-location", ipcSettings.inPort);
            }

            //If we have a cache, set the cache path
            if (cache)
            {
                cachePath ??= new FileInfo($"{browserEngineMainDir}/UWBCache");
                argsBuilder.AppendArgument("cache-path", cachePath.FullName, true);
            }

            //Setup web RTC
            if (webRtc)
                argsBuilder.AppendArgument("web-rtc", webRtc);

            //Setup remote debugging
            if (remoteDebugging)
                argsBuilder.AppendArgument("remote-debugging", remoteDebuggingPort);

            //Setup proxy
            argsBuilder.AppendArgument("proxy-server", proxySettings.ProxyServer);
            if (!string.IsNullOrWhiteSpace(proxySettings.Username))
                argsBuilder.AppendArgument("proxy-username", proxySettings.Username, true);

            if (!string.IsNullOrWhiteSpace(proxySettings.Password))
                argsBuilder.AppendArgument("proxy-password", proxySettings.Password, true);

            //Our engine active file
            argsBuilder.AppendArgument("active-engine-file-path", browserEngineMainDir, true);

            //Final built arguments
            string arguments = argsBuilder.ToString();

            //Setup communication manager
            communicationsManager = new WebBrowserCommunicationsManager(this);
            communicationsManager.Listen();

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
            serverProcess.OutputDataReceived += new ProcessLogHandler(this).HandleProcessLog;
            serverProcess.Start();
            serverProcess.BeginOutputReadLine();
            serverProcess.BeginErrorReadLine();

            WebBrowserUtils.WaitForActiveEngineFile(
                Path.GetFullPath($"{browserEngineMainDir}/{ActiveEngineFileName}"), engineStartupTimeout, () =>
                {
                    try
                    {
                        logger.Debug("UWB startup success, connecting...");
                        communicationsManager.Connect();
                        IsReady = true;
                    }
                    catch (Exception)
                    {
                        logger.Error("An error occured while connecting to the UWB process!");
                        Dispose();
                        throw;
                    }
                }, () =>
                {
                    logger.Error("The UWB engine failed to startup in time!");
                    Dispose();

                    throw new TimeoutException("The web browser engine failed to startup in time!");
                }).ConfigureAwait(false);
        }

        #region Main Loop

        private byte[] pixels;

        internal async Task PixelDataLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(25, cancellationToken);

                browserPixelDataMarker.Begin();
                pixels = communicationsManager.GetPixels().PixelData;
                browserPixelDataMarker.End();
            }
        }

        /// <summary>
        ///     Loads the pixel data into the <see cref="BrowserTexture"/>
        /// </summary>
        public void LoadTextureData()
        {
            if(!IsReady || !IsConnected)
                return;

            using (browserLoadTextureMarker.Auto())
            {
                if (pixels == null || pixels.Length == 0)
                    return;

                BrowserTexture.LoadRawTextureData(pixels);
                BrowserTexture.Apply(false);
            }
        }

        #endregion

        #region Browser Events

        /// <summary>
        ///     Invoked when the url changes
        /// </summary>
        public event OnUrlChangeDelegate OnUrlChanged;
        internal void InvokeUrlChanged(string url) => OnUrlChanged?.Invoke(url);

        /// <summary>
        ///     Invoked when the page starts to load
        /// </summary>
        public event OnLoadStartDelegate OnLoadStart;
        internal void InvokeLoadStart(string url) => OnLoadStart?.Invoke(url);

        /// <summary>
        ///     Invoked when the page finishes loading
        /// </summary>
        public event OnLoadFinishDelegate OnLoadFinish;
        internal void InvokeLoadFinish(string url) => OnLoadFinish?.Invoke(url);

        /// <summary>
        ///     Invoked when the title changes
        /// </summary>
        public event OnTitleChange OnTitleChange;
        internal void InvokeTitleChange(string title) => OnTitleChange?.Invoke(title);

        /// <summary>
        ///     Invoked when the loading progress changes
        ///     <para>Progress goes from 0 to 1</para>
        /// </summary>
        public event OnLoadingProgressChange OnLoadProgressChange;
        internal void InvokeLoadProgressChange(double progress) => OnLoadProgressChange?.Invoke(progress);

        /// <summary>
        ///     Invoked when the browser goes in or out of fullscreen
        /// </summary>
        public event OnFullscreenChange OnFullscreen;
        internal void InvokeFullscreen(bool fullscreen) => OnFullscreen?.Invoke(fullscreen);

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
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

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
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.SendMouseMoveEvent(new MouseMoveEvent
            {
                MouseX = (int)mousePos.x,
                MouseY = (int)mousePos.y
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
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.SendMouseClickEvent(new MouseClickEvent
            {
                MouseX = (int)mousePos.x,
                MouseY = (int)mousePos.y,
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
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

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
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.LoadUrl(url);
        }

        /// <summary>
        ///     Tells the browser to go forward
        /// </summary>
        internal void GoForward()
        {
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.GoForward();
        }

        /// <summary>
        ///     Tells the browser to go back
        /// </summary>
        internal void GoBack()
        {
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.GoBack();
        }

        /// <summary>
        ///     Refreshes the browser
        /// </summary>
        internal void Refresh()
        {
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.Refresh();
        }

        /// <summary>
        ///     Makes the browser load html
        /// </summary>
        /// <param name="html"></param>
        internal void LoadHtml(string html)
        {
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.LoadHtml(html);
        }

        /// <summary>
        ///     Executes JS in the browser
        /// </summary>
        /// <param name="js"></param>
        internal void ExecuteJs(string js)
        {
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.ExecuteJs(js);
        }

        internal void Resize(Resolution newResolution)
        {
            if(!IsReady)
                return;
            
            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");
            
            BrowserTexture.Reinitialize((int)newResolution.Width, (int)newResolution.Height);
            communicationsManager.Resize(newResolution);
        }

        #endregion

        #region Destroying

#if !UNITY_EDITOR
        ~WebBrowserClient()
        {
            ReleaseResources();
        }
#endif

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
            logger.Debug("UWB shutdown...");
            
            Object.Destroy(BrowserTexture);

            try
            {
                if (IsConnected)
                    communicationsManager.Shutdown();

                communicationsManager.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error($"Something failed while trying to shutdown the engine! Force shutting down. {ex}");
                serverProcess.KillTree();
                serverProcess.Dispose();
                serverProcess = null;
            }

            if (serverProcess != null && !IsReady && !IsConnected)
            {
                serverProcess.KillTree();
                serverProcess.Dispose();
                serverProcess = null;
                return;
            }
            
            IsReady = false;
            
            if(serverProcess is {HasExited: true})
                return;
            
            WaitForServerProcess().ConfigureAwait(false);
        }

        private async Task WaitForServerProcess()
        {
            await Task.Delay(5000);

            if (!serverProcess.HasExited)
            {
                serverProcess.KillTree();
                logger.Warn("Forced killed web browser process!");
            }

            serverProcess.Dispose();
        }

        #endregion
    }
}