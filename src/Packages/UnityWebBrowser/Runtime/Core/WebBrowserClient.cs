using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityWebBrowser.BrowserEngine;
using UnityWebBrowser.Communication;
using UnityWebBrowser.Events;
using UnityWebBrowser.Helper;
using UnityWebBrowser.Logging;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Events;
using Object = UnityEngine.Object;
using Resolution = UnityWebBrowser.Shared.Resolution;

namespace UnityWebBrowser.Core
{
    /// <summary>
    ///     The main object responsible for UWB.
    ///     <para>
    ///         This class handles:
    ///         <list type="bullet">
    ///             <item>UWB process setup</item>
    ///             <item>Texture setup and rendering</item>
    ///             <item>Wrapper for invoking methods on the UWB process</item>
    ///             <item>Shutdown</item>
    ///         </list>
    ///         If you need to do something with UWB, its probably here.
    ///     </para>
    /// </summary>
    [Serializable]
    public class WebBrowserClient : IDisposable
    {
        #region Profile Markers

        private static ProfilerMarker markerGetPixels = new("UWB.GetPixels");
        private static ProfilerMarker markerGetPixelsRpc = new("UWB.GetPixels.RPC");
        private static ProfilerMarker markerGetPixelsCopy = new("UWB.GetPixels.Copy");
        
        private static ProfilerMarker markerLoadTexture = new("UWB.LoadTexture");
        private static ProfilerMarker markerLoadTextureLoad = new("UWB.LoadTexture.LoadRawData");
        private static ProfilerMarker markerLoadTextureApply = new("UWB.LoadTexture.Apply");

        #endregion

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
        ///     The background <see cref="UnityEngine.Color32" /> of the webpage
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
        [Tooltip("Enable or disable WebRTC")] public bool webRtc;

        /// <summary>
        ///     Proxy Settings
        /// </summary>
        [Tooltip("Proxy settings")] public ProxySettings proxySettings;

        /// <summary>
        ///     Enable or disable remote debugging
        /// </summary>
        [Tooltip("Enable or disable remote debugging")]
        public bool remoteDebugging;

        /// <summary>
        ///     The port to use for remote debugging
        /// </summary>
        [Tooltip("The port to use for remote debugging")] [Range(1024, 65353)]
        public uint remoteDebuggingPort = 9022;

        /// <summary>
        ///     The <see cref="CommunicationLayer" /> to use
        /// </summary>
        [Header("IPC Settings")] [Tooltip("The communication layer to use")]
        public CommunicationLayer communicationLayer;

        /// <summary>
        ///     Timeout time for waiting for the engine to start (in milliseconds)
        /// </summary>
        [Tooltip("Timeout time for waiting for the engine to start (in markerGetPixelsRpcmilliseconds)")]
        public int engineStartupTimeout = 4000;

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
        public bool IsConnected => communicationsManager is {IsConnected: true};

        #region IsReady

        /// <summary>
        ///     The UWB engine has signaled that it is ready
        /// </summary>
        public bool ReadySignalReceived { get; internal set; }

        private object isReadyLock;
        private bool isReady;

        /// <summary>
        ///     Is everything for UWB ready? (Engine and client)
        /// </summary>
        public bool IsReady
        {
            // ReSharper disable InconsistentlySynchronizedField
            get
            {
                if (isReadyLock == null)
                    return isReady;

                lock (isReadyLock)
                {
                    return isReady;
                }
            }
            private set
            {
                if (isReadyLock == null)
                {
                    isReady = value;
                    return;
                }

                lock (isReadyLock)
                {
                    isReady = value;
                }
            }
            // ReSharper restore InconsistentlySynchronizedField
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
        ///     Internal usage of <see cref="IWebBrowserLogger" />
        /// </summary>
        internal IWebBrowserLogger logger = new DefaultUnityWebBrowserLogger();

        /// <summary>
        ///     Gets the <see cref="IWebBrowserLogger" /> to use for logging
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public IWebBrowserLogger Logger
        {
            get => logger;
            set => logger = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        private Process engineProcess;
        private WebBrowserCommunicationsManager communicationsManager;
        private CancellationTokenSource cancellationToken;

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

            //Check communication layer
            if (communicationLayer.IsInUse)
                throw new InitializationException("The communication layer is already in use!");
            communicationLayer.IsInUse = true;

            isReadyLock = new object();

            //Setup texture
            BrowserTexture = new Texture2D((int) resolution.Width, (int) resolution.Height, TextureFormat.BGRA32, false,
                false);
            WebBrowserUtils.SetAllTextureColorToOne(BrowserTexture, backgroundColor);
            pixelData = new NativeArray<byte>(new byte[(int) resolution.Width * (int) resolution.Height * 4], Allocator.Persistent);

            string browserEngineMainDir = WebBrowserUtils.GetBrowserEngineMainDirectory();

            //Start to build our arguments
            WebBrowserArgsBuilder argsBuilder = new();

            //Initial URL
            argsBuilder.AppendArgument("initial-url", initialUrl, true);

            //Width & Height
            argsBuilder.AppendArgument("width", resolution.Width);
            argsBuilder.AppendArgument("height", resolution.Height);

            //Javascript
            argsBuilder.AppendArgument("javascript", javascript);

            //Background color
            argsBuilder.AppendArgument("background-color", WebBrowserUtils.ColorToHex(backgroundColor));

            //Logging
            LogPath ??= new FileInfo($"{browserEngineMainDir}/{browserEngine}.log");
            argsBuilder.AppendArgument("log-path", LogPath.FullName, true);
            argsBuilder.AppendArgument("log-severity", logSeverity);

            //IPC settings
            communicationLayer.GetIpcSettings(out object outLocation, out object inLocation,
                out string assemblyLocation);
            if (assemblyLocation != null)
            {
                if (!File.Exists(assemblyLocation))
                {
                    logger.Error("Failed to find provided communication layer assembly!");
                    throw new FileNotFoundException("Failed to find provided communication layer assembly!");
                }

                argsBuilder.AppendArgument("comms-layer-path", assemblyLocation, true);
                logger.Debug($"Using communication layer assembly at '{assemblyLocation}'.");
            }

            argsBuilder.AppendArgument("in-location", inLocation, true);
            argsBuilder.AppendArgument("out-location", outLocation, true);

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

            //Final built arguments
            string arguments = argsBuilder.ToString();

            //Setup communication manager
            communicationsManager = new WebBrowserCommunicationsManager(this);
            communicationsManager.Listen();

            //Start the engine process
            try
            {
                engineProcess = WebBrowserUtils.CreateEngineProcess(browserEngine, browserEnginePath, arguments,
                    new ProcessLogHandler(this).HandleProcessLog);
            }
            catch (Exception ex)
            {
                logger.Error($"An error occured while setting up the engine process! {ex}");
                throw;
            }
            
            cancellationToken = new CancellationTokenSource();

            UniTask.Create(WaitForEngineReadyTask);
        }

        #region Readying

        /// <summary>
        ///     Will wait for <see cref="ReadySignalReceived" /> to be true
        /// </summary>
        internal async UniTask WaitForEngineReadyTask()
        {
            try
            {
                await UniTask.WaitUntil(() => ReadySignalReceived)
                    .Timeout(TimeSpan.FromMilliseconds(engineStartupTimeout));
            }
            catch (TimeoutException)
            {
                logger.Error("The engine did not get ready within engine startup timeout!");
                await using (UniTask.ReturnToMainThread())
                {
                    Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.Error($"An unknown error occured while waiting for engine to get ready! {ex}");
                await using (UniTask.ReturnToMainThread())
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        ///     Called when the engine sends the ready signal
        /// </summary>
        internal async UniTaskVoid EngineReady()
        {
            ReadySignalReceived = true;

            try
            {
                logger.Debug("UWB startup success, connecting...");
                communicationsManager.Connect();
                IsReady = true;
                _ = Task.Run(PixelDataLoop);
            }
            catch (Exception ex)
            {
                logger.Error($"An error occured while waiting to connect to the UWB engine process! {ex}");
                await using (UniTask.ReturnToMainThread())
                {
                    Dispose();
                }
            }
        }

        #endregion

        #region Main Loop
        
        private NativeArray<byte> pixelData;

        internal async Task PixelDataLoop()
        {
            CancellationToken token = cancellationToken.Token;
            while (!token.IsCancellationRequested)
                try
                {
                    if (!IsReady || !IsConnected)
                        continue;
                    
                    await Task.Delay(25, token);

                    if (token.IsCancellationRequested)
                        return;
                    
                    markerGetPixels.Begin();
                    {
                        markerGetPixelsRpc.Begin();
                        ReadOnlyMemory<byte> pixels = communicationsManager.GetPixels().PixelData;
                        markerGetPixelsRpc.End();
                        
                        //There can be a good amount of time between first getting the pixels and when we go to copy it to the native array
                        if(token.IsCancellationRequested)
                            return;
                        
                        markerGetPixelsCopy.Begin();
                        
                        WebBrowserUtils.CopySpanToNativeArray(pixels.Span, pixelData, token);
                        
                        markerGetPixelsCopy.End();
                    }
                    markerGetPixels.End();
                }
                catch (TaskCanceledException)
                {
                    //Do nothing
                }
                catch (Exception ex)
                {
                    logger.Error($"Error in data loop! {ex}");
                }
        }

        /// <summary>
        ///     Loads the pixel data into the <see cref="BrowserTexture" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LoadTextureData()
        {
            if (!IsReady || !IsConnected)
                return;

            using (markerLoadTexture.Auto())
            {
                if (pixelData.Length == 0)
                    return;
                
                Texture2D texture = BrowserTexture;
                markerLoadTextureLoad.Begin();
                texture.LoadRawTextureData(pixelData);
                markerLoadTextureLoad.End();
                
                markerLoadTextureApply.Begin();
                texture.Apply(false);
                markerLoadTextureApply.End();
            }
        }

        #endregion

        #region Browser Events

        /// <summary>
        ///     Invoked when the url changes
        /// </summary>
        public event OnUrlChangeDelegate OnUrlChanged;

        internal void InvokeUrlChanged(string url)
        {
            OnUrlChanged?.Invoke(url);
        }

        /// <summary>
        ///     Invoked when the page starts to load
        /// </summary>
        public event OnLoadStartDelegate OnLoadStart;

        internal void InvokeLoadStart(string url)
        {
            OnLoadStart?.Invoke(url);
        }

        /// <summary>
        ///     Invoked when the page finishes loading
        /// </summary>
        public event OnLoadFinishDelegate OnLoadFinish;

        internal void InvokeLoadFinish(string url)
        {
            OnLoadFinish?.Invoke(url);
        }

        /// <summary>
        ///     Invoked when the title changes
        /// </summary>
        public event OnTitleChange OnTitleChange;

        internal void InvokeTitleChange(string title)
        {
            OnTitleChange?.Invoke(title);
        }

        /// <summary>
        ///     Invoked when the loading progress changes
        ///     <para>Progress goes from 0 to 1</para>
        /// </summary>
        public event OnLoadingProgressChange OnLoadProgressChange;

        internal void InvokeLoadProgressChange(double progress)
        {
            OnLoadProgressChange?.Invoke(progress);
        }

        /// <summary>
        ///     Invoked when the browser goes in or out of fullscreen
        /// </summary>
        public event OnFullscreenChange OnFullscreen;

        internal void InvokeFullscreen(bool fullscreen)
        {
            OnFullscreen?.Invoke(fullscreen);
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
            if (!IsReady)
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
        public void SendMouseMove(Vector2 mousePos)
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

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
        public void SendMouseClick(Vector2 mousePos, int clickCount, MouseClickType clickType,
            MouseEventType eventType)
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

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
        public void SendMouseScroll(int mouseX, int mouseY, int mouseScroll)
        {
            if (!IsReady)
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
        public void LoadUrl(string url)
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.LoadUrl(url);
        }

        /// <summary>
        ///     Tells the browser to go forward
        /// </summary>
        public void GoForward()
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.GoForward();
        }

        /// <summary>
        ///     Tells the browser to go back
        /// </summary>
        public void GoBack()
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.GoBack();
        }

        /// <summary>
        ///     Refreshes the browser
        /// </summary>
        public void Refresh()
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.Refresh();
        }

        /// <summary>
        ///     Makes the browser load html
        /// </summary>
        /// <param name="html"></param>
        public void LoadHtml(string html)
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.LoadHtml(html);
        }

        /// <summary>
        ///     Executes JS in the browser
        /// </summary>
        /// <param name="js"></param>
        public void ExecuteJs(string js)
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            communicationsManager.ExecuteJs(js);
        }

        /// <summary>
        ///     Resizes the screen
        /// </summary>
        /// <param name="newResolution"></param>
        /// <exception cref="WebBrowserIsNotConnectedException"></exception>
        public void Resize(Resolution newResolution)
        {
            if (!IsReady)
                return;

            if (!IsConnected)
                throw new WebBrowserIsNotConnectedException("The Unity client is not connected to the browser engine!");

            BrowserTexture.Reinitialize((int) newResolution.Width, (int) newResolution.Height);
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
        ///     Has this object been disposed
        /// </summary>
        public bool HasDisposed { get; private set; }

        /// <summary>
        ///     Destroys this <see cref="WebBrowserClient" /> instance
        /// </summary>
        public void Dispose()
        {
            if (HasDisposed)
                return;

            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseResources()
        {
            if (HasDisposed)
                return;

            HasDisposed = true;
            logger.Debug("UWB shutdown...");

            cancellationToken?.Cancel();
            if (BrowserTexture != null)
                Object.Destroy(BrowserTexture);
            pixelData.Dispose();

            if (IsReady && IsConnected)
                communicationsManager.Shutdown();

            try
            {
                communicationsManager?.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error($"Some error occured while destroying the communications manager! {ex}");
            }

            communicationLayer.IsInUse = false;

            if (engineProcess != null)
            {
                if (IsReady)
                    engineProcess.KillTree();

                engineProcess.Dispose();
                engineProcess = null;
            }
        }

        #endregion
    }
}