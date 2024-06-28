// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using UnityWebBrowser.Engine.Cef.Shared.Browser;
using VoltstroStudios.UnityWebBrowser.Engine.Shared;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Popups;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Shared.Events;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core;

/// <summary>
///     Manager for CEF
/// </summary>
internal class CefEngineControlsManager : IEngineControls, IDisposable
{
    private readonly ILogger mainLogger;
    private readonly ILogger browserConsoleLogger;
    
    private string[] args;
    private UwbCefApp cefApp;

    private UwbCefClient cefClient;
    private CefMainArgs cefMainArgs;
    private LaunchArguments launchArguments;

    /// <summary>
    ///     Creates a new <see cref="CefEngineControlsManager" /> instance
    /// </summary>
    /// <exception cref="DllNotFoundException"></exception>
    /// <exception cref="CefVersionMismatchException"></exception>
    /// <exception cref="Exception"></exception>
    public CefEngineControlsManager(LoggerManager loggerManagerManager)
    {
        //Setup CEF
        CefRuntime.Load();

        //this.loggerManager = loggerManager;
        mainLogger = loggerManagerManager.CreateLogger("CEF Engine");
        browserConsoleLogger = loggerManagerManager.CreateLogger("CEF Engine Browser Console");
        
        CefLoggerWrapper.Init(mainLogger);
    }

    /// <summary>
    ///     Does early init stuff
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="rawArguments"></param>
    public void EarlyInit(LaunchArguments arguments, string[] rawArguments)
    {
        launchArguments = arguments;
        args = rawArguments;

        // ReSharper disable once RedundantAssignment
        string[] argv = args;
#if LINUX || MACOS
        //On Linux we need to do this, otherwise it will just crash, no idea why tho
        argv = new string[args.Length + 1];
        Array.Copy(args, 0, argv, 1, args.Length);
        argv[0] = "-";
#endif

        //Set up CEF args and the CEF app
        cefMainArgs = new CefMainArgs(argv);
        cefApp = new UwbCefApp(launchArguments);
    }

    /// <summary>
    ///     Starts CEF
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Init(ClientControlsActions clientControlsActions, EnginePopupManager popupManager)
    {
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
            PersistSessionCookies = true,
            PersistUserPreferences = true,
            ResourcesDirPath = Path.Combine(Environment.CurrentDirectory),
            LocalesDirPath = Path.Combine(Environment.CurrentDirectory, "locales"),
            BrowserSubprocessPath = Path.Combine(Environment.CurrentDirectory, "UnityWebBrowser.Engine.Cef.SubProcess.exe")
        };

        //Init CEF
        CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

        //Create a CEF window and set it to windowless
        CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
        cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

        //Create our CEF browser settings
        Color suppliedColor = launchArguments.BackgroundColor;
        CefColor backgroundColor = new(suppliedColor.A, suppliedColor.R, suppliedColor.G, suppliedColor.B);
        CefBrowserSettings cefBrowserSettings = new()
        {
            BackgroundColor = backgroundColor,
            JavaScript = launchArguments.JavaScript ? CefState.Enabled : CefState.Disabled,
            LocalStorage = launchArguments.LocalStorage ? CefState.Enabled : CefState.Disabled
        };

        mainLogger.LogDebug($"Starting CEF with these options:" +
                     $"\nProcess Path: {Environment.ProcessPath}" +
                     $"\nJS: {launchArguments.JavaScript}" +
                     $"\nLocal Storage: {launchArguments.LocalStorage}" +
                     $"\nBackgroundColor: {suppliedColor}" +
                     $"\nCache Path: {cachePathArgument}" +
                     $"\nPopup Action: {launchArguments.PopupAction}" +
                     $"\nLog Path: {launchArguments.LogPath.FullName}" +
                     $"\nLog Severity: {launchArguments.LogSeverity}");
        mainLogger.LogInformation($"Starting CEF client...");

        //Create cef browser
        cefClient = new UwbCefClient(
            new CefSize(launchArguments.Width, launchArguments.Height),
            launchArguments.PopupAction,
            popupManager,
            new ProxySettings(launchArguments.ProxyUsername, launchArguments.ProxyPassword, launchArguments.ProxyEnabled),
            clientControlsActions,
            mainLogger,
            browserConsoleLogger);
        CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, launchArguments.InitialUrl);
    }

    #region Engine Actions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PixelsEvent GetPixels()
    {
        return new PixelsEvent
        {
            PixelData = cefClient.GetPixels()
        };
    }

    public void Shutdown()
    {
        //We can only quit the message loop on the UI (main) thread
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefActionTask.PostTask(CefThreadId.UI, Shutdown);
            return;
        }

        mainLogger.LogDebug($"Quitting message loop...");
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

    public Vector2 GetScrollPosition()
    {
        return cefClient.GetMouseScrollPosition();
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

    public void SetZoomLevel(double zoomLevel)
    {
        cefClient.SetZoomLevel(zoomLevel);
    }

    public double GetZoomLevel()
    {
        return cefClient.GetZoomLevel();
    }

    public void OpenDevTools()
    {
        cefClient.OpenDevTools();
    }

    public void Resize(Resolution resolution)
    {
        cefClient.Resize(resolution);
    }

    #endregion

    #region Destroy

    ~CefEngineControlsManager()
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