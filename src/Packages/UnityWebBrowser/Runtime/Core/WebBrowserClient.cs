// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

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
using VoltstroStudios.UnityWebBrowser.Communication;
using VoltstroStudios.UnityWebBrowser.Core.Engines;
using VoltstroStudios.UnityWebBrowser.Core.Js;
using VoltstroStudios.UnityWebBrowser.Core.Popups;
using VoltstroStudios.UnityWebBrowser.Events;
using VoltstroStudios.UnityWebBrowser.Helper;
using VoltstroStudios.UnityWebBrowser.Logging;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Events;
using VoltstroStudios.UnityWebBrowser.Shared.Js;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;
using Object = UnityEngine.Object;
using Resolution = VoltstroStudios.UnityWebBrowser.Shared.Resolution;

namespace VoltstroStudios.UnityWebBrowser.Core
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

        internal static ProfilerMarker markerGetPixels = new("UWB.GetPixels");
        internal static ProfilerMarker markerGetPixelsRpc = new("UWB.GetPixels.RPC");

        internal static ProfilerMarker markerLoadTextureApply = new("UWB.LoadTexture.Apply");

        #endregion

        /// <summary>
        ///     The active browser engine this instance is using
        /// </summary>
        [Header("Browser Settings")] [Tooltip("The active browser engine this instance is using")]
        public Engine engine;

        /// <summary>
        ///     The initial URl the browser will start at
        /// </summary>
        [Tooltip("The initial URl the browser will start at")]
        public string initialUrl = "https://voltstro.dev";

        #region Resoltuion

        [SerializeField] private Resolution resolution = new(1920, 1080);

        /// <summary>
        ///     The resolution of the browser.
        ///     <para>There is a chance that resizing the screen causes UWB to crash Unity, use carefully!</para>
        ///     <para>Resizing in performance mode is not supported!</para>
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
        [Obsolete("Cache control is no longer used. A cache path will always be used now. To use a incognito/private mode, where no profile-specific data is persisted to disk, set incognitoMode to true.")]
        [HideInInspector]
        public bool cache = true;

        /// <summary>
        ///     Enable or disable incognito/private mode.
        ///     When true, no profile-specific data is persisted to disk, but cache is sill used to persist installation-specific data.
        /// </summary>
        [Tooltip("Enable or disable incognito/private mode. When true, no profile-specific data is persisted to disk, but cache is still used to persist installation-specific data.")]
        public bool incognitoMode;

        /// <summary>
        ///     Enable or disable local storage
        /// </summary>
        [Tooltip("Enable or disable local storage")]
        public bool localStorage = true;

        /// <summary>
        ///     How to handle popups
        /// </summary>
        [Tooltip("How to handle popups")] public PopupAction popupAction;

        /// <summary>
        ///     Proxy Settings
        /// </summary>
        [Tooltip("Proxy settings")] public ProxySettings proxySettings;
        
        /// <summary>
        ///     Target framerate for the browser rendering (1-60 FPS).
        ///     Higher values provide smoother video playback but use more CPU.
        /// </summary>
        [Header("Performance")]
        [Tooltip("Target framerate for browser rendering (1-60). Higher = smoother but more CPU.")]
        [Range(1, 60)]
        public int windowlessFrameRate = 30;

        /// <summary>
        ///     Enable or disable WebRTC
        /// </summary>
        [Header("Advanced")]
        [Tooltip("Enable or disable WebRTC")] public bool webRtc;

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
        ///     Origins that are allowed to access remote debugging when <see cref="remoteDebugging"/> is enabled
        /// </summary>
        [Tooltip("Origins that are allowed to access remote debugging when remoteDeubgging is enabled")]
        public string[] remoteDebuggingAllowedOrigins = new[] { "http://127.0.0.1:9022" };
        
        /// <summary>
        ///     Manager for JS methods
        /// </summary>
        [Tooltip("Manager for JS methods")]
        public JsMethodManager jsMethodManager = new();

        /// <summary>
        ///     Will ignore SSL errors on provided domains in <see cref="ignoreSslErrorsDomains"/>
        /// </summary>
        [Tooltip("Will ignore SSL errors on provided domains in ignoreSSLErrorsDomains")]
        public bool ignoreSslErrors = false;

        /// <summary>
        ///     Disables sandbox
        /// </summary>
        [Tooltip("Disables sandbox")]
        public bool noSandbox = false;

        /// <summary>
        ///     Domains to ignore SSL errors on if <see cref="ignoreSslErrors"/> is enabled
        /// </summary>
        [Tooltip("Domains to ignore SSL errors on if ignoreSSLErrors is enabled")]
        public string[] ignoreSslErrorsDomains;

        /// <summary>
        ///     The <see cref="CommunicationLayer" /> to use
        /// </summary>
        [Header("IPC Settings")] [Tooltip("The communication layer to use")]
        public CommunicationLayer communicationLayer;

        /// <summary>
        ///     Timeout time for waiting for the engine to start (in milliseconds)
        /// </summary>
        [Tooltip("Timeout time for waiting for the engine to start (in milliseconds)")]
        public int engineStartupTimeout = 4000;

        /// <summary>
        ///     The log severity. Only messages of this severity level or higher will be logged
        /// </summary>
        [Tooltip("The log severity. Only messages of this severity level or higher will be logged")]
        public LogSeverity logSeverity = LogSeverity.Info;
        
        /// <summary>
        ///     Ignores errors related to log messages from the engine process being in a non-json format
        /// </summary>
        [Tooltip("Ignores errors related to log messages from the engine process being in a non-json format")]
        public bool ignoreLogProcessJsonErrors = true;

        /// <summary>
        ///     Texture that the browser will paint to.
        ///     <para>In headless mode this will be null</para>
        /// </summary>
        public Texture2D BrowserTexture { get; private set; }

        /// <summary>
        ///     Are we connected to the UW engine process
        /// </summary>
        public bool IsConnected => communicationsManager is { IsConnected: true };

        /// <summary>
        ///     The UWB engine has signaled that it is ready
        /// </summary>
        public bool ReadySignalReceived { get; internal set; }
        
        /// <summary>
        ///     Has UWB initialized
        /// </summary>
        public bool HasInitialized { get; internal set; }

        /// <summary>
        ///     Internal FPS of pixels communication between Unity and the Engine
        /// </summary>
        public int FPS { get; private set; }

        #region Log Path

        private FileInfo logPath;

        /// <summary>
        ///     The path that UWB engine will log to
        /// </summary>
        /// <exception cref="UwbHasInitializedException">Thrown if value is attempted to be changed once UWB has already initialized</exception>
        /// <exception cref="ArgumentNullException">Thrown is value provide is null</exception>
        public FileInfo LogPath
        {
            get => logPath;
            set
            {
                if (HasInitialized)
                    throw new UwbHasInitializedException("You cannot change the log path once UWB has initialized!");

                logPath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        #region Cache Path

        private FileInfo cachePath;

        /// <summary>
        ///     The path to the cache
        /// </summary>
        /// <exception cref="UwbHasInitializedException">Thrown if value is attempted to be changed once UWB has already initialized</exception>
        /// <exception cref="ArgumentNullException">Thrown is value provide is null</exception>
        public FileInfo CachePath
        {
            get => cachePath;
            set
            {
                if (HasInitialized)
                    throw new UwbHasInitializedException("You cannot change the cache path once UWB has initialized!");

                cachePath = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        #region Logger

        public ProcessLogHandler processLogHandler;

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

        private bool headless;
        private bool hasInitialized;
        private EngineProcess engineProcess;
        private WebBrowserCommunicationsManager communicationsManager;
        private CancellationTokenSource cancellationSource;

        private object resizeLock;
        private NativeArray<byte> textureData;
        internal NativeArray<byte> nextTextureData;

        /// <summary>
        ///     Creates a new <see cref="WebBrowserClient"/> instance
        /// </summary>
        /// <param name="headless">
        ///     Creates the browser client in headless mode.
        ///     Headless mode will not create a <see cref="BrowserTexture"/> for you to use
        /// </param>
        public WebBrowserClient(bool headless = false)
        {
            this.headless = headless;
        }

        /// <summary>
        ///     Inits the browser client.
        ///     In normal operation you do not need to call this method.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InitializationException"></exception>
        public void Init()
        {
            //Initialized check
            if (hasInitialized)
                throw new InitializationException("The browser client has already been initialized!");

            hasInitialized = true;
            
            //OS support check
            if (!WebBrowserUtils.IsRunningOnSupportedPlatform())
            {
                logger.Warn("UWB is not supported on the current runtime platform! Not running.");
                Dispose();
                return;
            }
            
            //Get the path to the UWB process we are using and make sure it exists
            string browserEnginePath = engine.GetEngineAppPath(WebBrowserUtils.GetRunningPlatform());
            logger.Debug($"Starting browser engine process from '{browserEnginePath}'...");

            if (!File.Exists(browserEnginePath))
            {
                logger.Error("The engine process could not be found!");
                throw new FileNotFoundException("The engine process could not be found!");
            }

            //Check communication layer
            if (communicationLayer.IsInUse)
                throw new InitializationException("The communication layer is already in use!");
            communicationLayer.IsInUse = true;

            //Setup texture
            if (!headless)
            {
                BrowserTexture = new Texture2D((int)resolution.Width, (int)resolution.Height, TextureFormat.BGRA32, false,
                    false);
                WebBrowserUtils.SetAllTextureColorToOne(BrowserTexture, backgroundColor);
                resizeLock = new object();
                textureData = BrowserTexture.GetRawTextureData<byte>();
                nextTextureData = new NativeArray<byte>(textureData.ToArray(), Allocator.Persistent);
            }
            
            string browserEngineMainDir = WebBrowserUtils.GetAdditionFilesDirectory();

            //Start to build our arguments
            WebBrowserArgsBuilder argsBuilder = new();

            //Initial URL
            argsBuilder.AppendArgument("initial-url", initialUrl, true);

            //Width & Height
            argsBuilder.AppendArgument("width", resolution.Width);
            argsBuilder.AppendArgument("height", resolution.Height);

            //Windowless frame rate
            argsBuilder.AppendArgument("windowless-frame-rate", windowlessFrameRate);

            //Javascript
            argsBuilder.AppendArgument("javascript", javascript);

            //Background color
            argsBuilder.AppendArgument("background-color", WebBrowserUtils.ColorToHex(backgroundColor));

            //Logging
            LogPath ??= new FileInfo(Path.Combine(browserEngineMainDir, $"{Path.GetFileNameWithoutExtension(engine.GetEngineExecutableName())}.log"));
            argsBuilder.AppendArgument("log-path", LogPath.FullName, true);
            argsBuilder.AppendArgument("log-severity", logSeverity);

            //IPC settings
            communicationLayer.GetIpcSettings(out object outLocation, out object inLocation,
                out string assemblyLocation);
            if (assemblyLocation != null)
            {
                argsBuilder.AppendArgument("comms-layer-name", assemblyLocation, true);
                logger.Debug($"Using communication layer of '{assemblyLocation}'.");
            }

            argsBuilder.AppendArgument("in-location", inLocation, true);
            argsBuilder.AppendArgument("out-location", outLocation, true);

            //Set cache path
            cachePath ??= new FileInfo(Path.Combine(browserEngineMainDir, "UWBCache"));
            argsBuilder.AppendArgument("cache-path", cachePath.FullName, true);
            
            argsBuilder.AppendArgument("incognito-mode", incognitoMode);

            //Popups
            argsBuilder.AppendArgument("popup-action", popupAction, true);

            //Setup web RTC
            if (webRtc)
                argsBuilder.AppendArgument("web-rtc", webRtc);

            argsBuilder.AppendArgument("local-storage", localStorage);

            //Setup remote debugging
            if (remoteDebugging)
            {
                argsBuilder.AppendArgument("remote-debugging", remoteDebuggingPort);
                argsBuilder.AppendArgument("remote-debugging-allowed-origins", string.Join(",", remoteDebuggingAllowedOrigins));
            }

            //Setup proxy
            if (proxySettings.ProxyServer)
            {
                argsBuilder.AppendArgument("proxy-server", true);
                if (!string.IsNullOrWhiteSpace(proxySettings.Username))
                    argsBuilder.AppendArgument("proxy-username", proxySettings.Username, true);

                if (!string.IsNullOrWhiteSpace(proxySettings.Password))
                    argsBuilder.AppendArgument("proxy-password", proxySettings.Password, true);
            }
            
            //Ignore ssl errors
            if (ignoreSslErrors)
            {
                argsBuilder.AppendArgument("ignore-ssl-errors", true);
                argsBuilder.AppendArgument("ignore-ssl-errors-domains", string.Join(",", ignoreSslErrorsDomains));
            }

            //Disable sandbox
            if (noSandbox)
            {
                argsBuilder.AppendArgument("no-sandbox", true);
            }

            //Final built arguments
            string arguments = argsBuilder.ToString();

            //Setup communication manager
            cancellationSource = new CancellationTokenSource();
            communicationsManager = new WebBrowserCommunicationsManager(this, cancellationSource);
            communicationsManager.Listen();
            
#if UNITY_EDITOR
            //Install reload events handler
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif
            
            //Mark has initialized and invoke event
            HasInitialized = true;
            try
            {
                OnClientInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                logger.Error($"Error invoking OnClientInitialized! {ex}");
            }

            //Start the engine process
            UniTask.Create(() => 
                 StartEngineProcess(arguments))
                .ContinueWith(() => WaitForEngineReadyTask(cancellationSource.Token))
                .Forget();
        }

        #region Starting

        private UniTask StartEngineProcess(string engineProcessArguments)
        {
            try
            {
                //TODO: Move process log handler to engine process class
                processLogHandler = new ProcessLogHandler(this);
                engineProcess = new EngineProcess(engine, logger);
                engineProcess.StartProcess(engineProcessArguments, processLogHandler.HandleOutputProcessLog, processLogHandler.HandleErrorProcessLog);
            }
            catch (Exception ex)
            {
                logger.Error($"An error occured while setting up the engine process! {ex}");
                throw;
            }

            return UniTask.CompletedTask;
        }

        /// <summary>
        ///     Will wait for <see cref="ReadySignalReceived" /> to be true
        /// </summary>
        internal async UniTask WaitForEngineReadyTask(CancellationToken cancellationToken)
        {
            try
            {
                //Wait until we get a ready signal, or timeout
                await UniTask.WaitUntil(() => 
                        ReadySignalReceived, cancellationToken: cancellationToken)
                    .Timeout(TimeSpan.FromMilliseconds(engineStartupTimeout));
            }
            catch (TimeoutException)
            {
                logger.Error(engineProcess.HasExited
                    ? $"The engine did not get ready within engine startup timeout! The engine process is not even running! Exit code: {engineProcess.ExitCode}."
                    : "The engine did not get ready within engine startup timeout! Try increasing your 'Engine Startup Timeout' time. If you continue to have this error, see issue report #166 on GitHub.");
                await using (UniTask.ReturnToMainThread())
                {
                    Dispose();
                }
            }
            catch (OperationCanceledException)
            {
                //Token probs got canceled
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

                //Fire OnClientConnected on main thread
                await using (UniTask.ReturnToMainThread())
                {
                    try
                    {
                        OnClientConnected?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        //This shouldn't ever happen
                        logger.Warn($"An error occured invoking OnClientConnected! {ex}");
                    }
                }

                //Create pixel data loop in headless mode
                if(headless)
                    return;
                
                Thread pixelDataLoopThread = new(PixelDataLoop)
                {
                    Name = "UWB Pixel Data Loop Thread"
                };
                pixelDataLoopThread.Start();
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

        internal void PixelDataLoop()
        {
            CancellationToken token = cancellationSource.Token;
            while (!token.IsCancellationRequested)
                try
                {
                    if (!IsConnected)
                        continue;

                    if (engineProcess.HasExited)
                    {
                        logger.Error("It appears that the engine process has quit!");
                        cancellationSource.Cancel();
                        return;
                    }

                    Thread.Sleep(5);

                    if (token.IsCancellationRequested)
                        return;

                    markerGetPixels.Begin();
                    {
                        lock (resizeLock)
                        {
                            markerGetPixelsRpc.Begin();
                            {
                                communicationsManager.GetPixels();
                            }
                            markerGetPixelsRpc.End();

                            textureData.CopyFrom(nextTextureData);
                        }
                    }
                    markerGetPixels.End();

                    frames++;
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
            if (!IsConnected)
                return;

            Texture2D texture = BrowserTexture;

            markerLoadTextureApply.Begin();
            texture.Apply(false);
            markerLoadTextureApply.End();
        }

        #region FPS

        private int frames;
        private float lastUpdateTime;

        /// <summary>
        ///     Updates FPS values
        ///     <para>Normal usage shouldn't require invoking this</para>
        /// </summary>
        public void UpdateFps()
        {
            float currentTime = Time.time;
            if (currentTime - lastUpdateTime > 1)
            {
                lastUpdateTime = currentTime;
                FPS = frames;
                frames = 0;
            }
        }

        #endregion

        #endregion

        #region Browser Events

        /// <summary>
        ///     Invoked when this <see cref="WebBrowserClient"/> initalizes.
        ///
        ///     <para>Initialized does not mean that the engine is ready, for that, use <see cref="OnClientConnected"/></para>
        /// </summary>
        public event OnClientInitialized OnClientInitialized;

        /// <summary>
        ///     Invoked when this <see cref="WebBrowserClient"/> connects to the engine
        /// </summary>
        public event OnClientConnected OnClientConnected;

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

        /// <summary>
        ///     Invoked when the browser gets a popup
        /// </summary>
        public event OnPopup OnPopup;

        internal void InvokeOnPopup(WebBrowserPopupInfo popupInfo)
        {
            OnPopup?.Invoke(popupInfo);
        }

        public event OnInputFocus OnInputFocus;

        internal void InvokeOnInputFocus(bool focused)
        {
            OnInputFocus?.Invoke(focused);
        }

        #endregion

        #region Browser Controls

        /// <summary>
        ///     Sends a keyboard event
        /// </summary>
        /// <param name="keysDown"></param>
        /// <param name="keysUp"></param>
        /// <param name="chars"></param>
        public void SendKeyboardControls(WindowsKey[] keysDown, WindowsKey[] keysUp, char[] chars)
        {
            CheckIfIsReadyAndConnected();

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
            CheckIfIsReadyAndConnected();

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
        public void SendMouseClick(Vector2 mousePos, int clickCount, MouseClickType clickType,
            MouseEventType eventType)
        {
            CheckIfIsReadyAndConnected();

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
        /// <param name="mousePos"></param>
        /// <param name="mouseScroll"></param>
        public void SendMouseScroll(Vector2 mousePos, int mouseScroll)
        {
            CheckIfIsReadyAndConnected();

            communicationsManager.SendMouseScrollEvent(new MouseScrollEvent
            {
                MouseX = (int)mousePos.x,
                MouseY = (int)mousePos.y,
                MouseScroll = mouseScroll
            });
        }

        /// <summary>
        ///     Tells the browser to load a URL
        /// </summary>
        /// <param name="url"></param>
        public void LoadUrl(string url)
        {
            CheckIfIsReadyAndConnected();

            communicationsManager.LoadUrl(url);
        }

        /// <summary>
        ///     Gets the mouse scroll position
        ///     <para>THIS IS INVOKED ON THE THREAD THAT IS CALLING THIS AND IS BLOCKING</para>
        /// </summary>
        /// <returns>Returns the mouse scroll position as a <see cref="Vector2" /></returns>
        public Vector2 GetScrollPosition()
        {
            CheckIfIsReadyAndConnected();

            //Gotta convert it to a Unity vector2
            System.Numerics.Vector2 position = communicationsManager.GetScrollPosition();
            return new Vector2(position.X, position.Y);
        }

        /// <summary>
        ///     Tells the browser to go forward
        /// </summary>
        public void GoForward()
        {
            CheckIfIsReadyAndConnected();

            communicationsManager.GoForward();
        }

        /// <summary>
        ///     Tells the browser to go back
        /// </summary>
        public void GoBack()
        {
            CheckIfIsReadyAndConnected();

            communicationsManager.GoBack();
        }

        /// <summary>
        ///     Refreshes the browser
        /// </summary>
        public void Refresh()
        {
            CheckIfIsReadyAndConnected();

            communicationsManager.Refresh();
        }

        /// <summary>
        ///     Makes the browser load html
        /// </summary>
        /// <param name="html"></param>
        public void LoadHtml(string html)
        {
            CheckIfIsReadyAndConnected();

            communicationsManager.LoadHtml(html);
        }

        /// <summary>
        ///     Executes JS in the browser
        /// </summary>
        /// <param name="js"></param>
        public void ExecuteJs(string js)
        {
            CheckIfIsReadyAndConnected();

            communicationsManager.ExecuteJs(js);
        }

        /// <summary>
        ///     Sets zoom level based off a percentage
        /// </summary>
        /// <param name="percent"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if percent is 0 or less</exception>
        public void SetZoomLevelPercent(double percent)
        {
            if (percent <= 0)
                throw new ArgumentOutOfRangeException(nameof(percent),
                    "Percent must be larger then 0. To reset, use SetZoomLevel(0).");
            
            //Logic from:
            //https://magpcss.org/ceforum/viewtopic.php?t=11491
            double scale = 1.2 * percent;
            double zoomLevel = Math.Log(scale);
            SetZoomLevel(zoomLevel);
        }

        /// <summary>
        ///     Set browser's zoom level. Use 0.0 to reset.
        /// </summary>
        /// <param name="zoomLevel"></param>
        public void SetZoomLevel(double zoomLevel)
        {
            CheckIfIsReadyAndConnected();
            
            communicationsManager.SetZoomLevel(zoomLevel);
        }
        
        /// <summary>
        ///     Get's browser's zoom level
        /// </summary>
        /// <returns></returns>
        public double GetZoomLevel()
        {
            CheckIfIsReadyAndConnected();

            return communicationsManager.GetZoomLevel();
        }

        /// <summary>
        ///     Shows dev tools
        /// </summary>
        public void OpenDevTools()
        {
            CheckIfIsReadyAndConnected();
            
            communicationsManager.OpenDevTools();
        }

        /// <summary>
        ///     Resizes the screen.
        /// </summary>
        /// <param name="newResolution"></param>
        /// <exception cref="UwbIsNotConnectedException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public void Resize(Resolution newResolution)
        {
            CheckIfIsReadyAndConnected();

            if (!headless)
            {
                lock (resizeLock)
                {
                    BrowserTexture.Reinitialize((int)newResolution.Width, (int)newResolution.Height);
                    textureData = BrowserTexture.GetRawTextureData<byte>();

                    nextTextureData.Dispose();
                    nextTextureData = new NativeArray<byte>(textureData.ToArray(), Allocator.Persistent);
                    communicationsManager.pixelsEventTypeReader.SetPixelDataArray(nextTextureData);
                }
            }
            
            communicationsManager.Resize(newResolution);
            logger.Debug($"Resized to {newResolution}.");
        }

        /// <summary>
        ///     Mutes browser audio
        /// </summary>
        /// <param name="muted"></param>
        public void AudioMute(bool muted)
        {
            CheckIfIsReadyAndConnected();
            
            communicationsManager.AudioMute(muted);
        }

        [DebuggerStepThrough]
        private void CheckIfIsReadyAndConnected()
        {
            if (!ReadySignalReceived)
                throw new UwbIsNotReadyException("UWB is not currently ready!");

            if (!IsConnected)
                throw new UwbIsNotConnectedException("UWB is not currently connected!");
        }

        #endregion

        #region JS Methods
        
        /// <summary>
        ///     Registers a method with <see cref="JsMethodManager"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterJsMethod(string name, Action method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            jsMethodManager.RegisterJsMethod(name, method.Method, method.Target);
        }
        
        /// <summary>
        ///     Registers a method with <see cref="JsMethodManager"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterJsMethod<T>(string name, Action<T> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            jsMethodManager.RegisterJsMethod(name, method.Method, method.Target);
        }
        
        /// <summary>
        ///     Registers a method with <see cref="JsMethodManager"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterJsMethod<T1, T2>(string name, Action<T1, T2> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            jsMethodManager.RegisterJsMethod(name, method.Method, method.Target);
        }
        
        /// <summary>
        ///     Registers a method with <see cref="JsMethodManager"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterJsMethod<T1, T2, T3>(string name, Action<T1, T2, T3> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            jsMethodManager.RegisterJsMethod(name, method.Method, method.Target);
        }
        
        /// <summary>
        ///     Registers a method with <see cref="JsMethodManager"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterJsMethod<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            jsMethodManager.RegisterJsMethod(name, method.Method, method.Target);
        }
        
        /// <summary>
        ///     Registers a method with <see cref="JsMethodManager"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterJsMethod<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            jsMethodManager.RegisterJsMethod(name, method.Method, method.Target);
        }
        
        /// <summary>
        ///     Registers a method with <see cref="JsMethodManager"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterJsMethod<T1, T2, T3, T4, T5, T6>(string name, Action<T1, T2, T3, T4, T5, T6> method)
        {
            if(method == null)
                throw new ArgumentNullException(nameof(method));
            
            jsMethodManager.RegisterJsMethod(name, method.Method, method.Target);
        }
        
        internal void InvokeJsMethod(ExecuteJsMethod executeJsMethod)
        {
            jsMethodManager.InvokeJsMethod(executeJsMethod);
        }

        #endregion

        #region Destroying

#if UNITY_EDITOR
        private void OnBeforeAssemblyReload()
        {
            if (HasInitialized && !HasDisposed)
            {
                logger.Warn("UWB is shutting down due to incoming domain reload. UWB does not support domain reloading while running.");
                Dispose();
            }
        }
#endif

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

            cancellationSource?.Cancel();

            //Destroy textures
            if (BrowserTexture != null)
                Object.Destroy(BrowserTexture);

            //Engine shutdown
            try
            {
                if (ReadySignalReceived && IsConnected)
                    communicationsManager.Shutdown();
            }
            catch (Exception ex)
            {
                logger.Error($"Some error occured while shutting down the engine! {ex}");
            }

            //Communication manager destruction
            try
            {
                communicationsManager?.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error($"Some error occured while destroying the communications manager! {ex}");
            }

            //We are no longer using our communication manager
            if (communicationLayer != null)
                communicationLayer.IsInUse = false;

            //Kill the process if we haven't already
            if (engineProcess != null)
            {
                if (!engineProcess.HasExited)
                    engineProcess.KillProcess();
                    
                engineProcess.Dispose();
                engineProcess = null;
            }
            
#if UNITY_EDITOR
            //Install reload events handler
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
#endif

            //Dispose of buffers
            if (resizeLock == null)
                return;
            
            lock (resizeLock)
            {
                if (nextTextureData.IsCreated)
                    nextTextureData.Dispose();
            }
        }

        #endregion
    }
}
