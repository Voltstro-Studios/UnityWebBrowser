// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using UnityWebBrowser.Engine.Cef.Shared.Browser.Js;
using UnityWebBrowser.Engine.Cef.Shared.Browser.Messages;
using UnityWebBrowser.Engine.Cef.Shared.Browser.Popups;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Popups;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Events;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser;

/// <summary>
///     Offscreen CEF
/// </summary>
internal class UwbCefClient : CefClient, IDisposable
{
    public readonly ClientControlsActions ClientControls;

    private readonly UwbCefContextMenuHandler contextMenuHandler;
    private readonly UwbCefDisplayHandler displayHandler;
    private readonly UwbCefLifespanHandler lifespanHandler;

    private readonly UwbCefLoadHandler loadHandler;

    private readonly ILogger mainLogger;

    private readonly ProxySettings proxySettings;
    private readonly UwbCefRenderHandler renderHandler;
    private readonly UwbCefRequestHandler requestHandler;

    private CefBrowser browser;
    private CefBrowserHost browserHost;
    private CefBrowserSettings devToolsBrowserSettings;
    private UwbCefPopupClient devToolsClient;

    //Dev Tools
    private CefWindowInfo devToolsWindowInfo;

    /// <summary>
    ///     Creates a new <see cref="UwbCefClient" /> instance
    /// </summary>
    public UwbCefClient(
        CefSize size,
        PopupAction popupAction,
        EnginePopupManager popupManager,
        ProxySettings proxySettings,
        ClientControlsActions clientControlsActions,
        ILogger mainLogger,
        ILogger browserConsoleLogger)
    {
        ClientControls = clientControlsActions;

        this.proxySettings = proxySettings;

        this.mainLogger = mainLogger;

        //Setup our handlers
        loadHandler = new UwbCefLoadHandler(this);
        renderHandler = new UwbCefRenderHandler(this, size);
        lifespanHandler = new UwbCefLifespanHandler(popupAction, popupManager, proxySettings);
        lifespanHandler.AfterCreated += cefBrowser =>
        {
            browser = cefBrowser;
            browserHost = cefBrowser.GetHost();
        };
        displayHandler = new UwbCefDisplayHandler(this, mainLogger, browserConsoleLogger);
        requestHandler = new UwbCefRequestHandler(proxySettings);
        contextMenuHandler = new UwbCefContextMenuHandler();

        //Create message types
        messageTypes = new Dictionary<string, IMessageBase>
        {
            [ExecuteJsMethodMessage.ExecuteJsMethodName] = new ExecuteJsMethodMessage(clientControlsActions)
        };
    }

    /// <summary>
    ///     Destroys the <see cref="UwbCefClient" /> instance
    /// </summary>
    public void Dispose()
    {
        browserHost?.CloseBrowser(true);
        browserHost?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Gets the pixel data of the CEF window
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte> GetPixels()
    {
        return browserHost == null ? Array.Empty<byte>() : renderHandler.Pixels;
    }

    protected override CefLoadHandler GetLoadHandler()
    {
        return loadHandler;
    }

    protected override CefRenderHandler GetRenderHandler()
    {
        return renderHandler;
    }

    protected override CefLifeSpanHandler GetLifeSpanHandler()
    {
        return lifespanHandler;
    }

    protected override CefDisplayHandler GetDisplayHandler()
    {
        return displayHandler;
    }

    protected override CefRequestHandler GetRequestHandler()
    {
        return requestHandler;
    }

    protected override CefContextMenuHandler GetContextMenuHandler()
    {
        return contextMenuHandler;
    }

    #region Engine Events

    private CefEventFlags GetKeyDirection(WindowsKey key) => key switch
    {
        WindowsKey.LShiftKey | WindowsKey.LControlKey | WindowsKey.LMenu => CefEventFlags.IsLeft,
        WindowsKey.RShiftKey | WindowsKey.RControlKey | WindowsKey.RMenu => CefEventFlags.ShiftDown,
        _ => CefEventFlags.None
    };

    /// <summary>
    ///     Process a <see cref="KeyboardEvent" />
    /// </summary>
    /// <param name="keyboardEvent"></param>
    public void ProcessKeyboardEvent(KeyboardEvent keyboardEvent)
    {
        UpdateModifiers(keyboardEvent);

        //Keys down
        foreach (WindowsKey key in keyboardEvent.KeysDown)
        {
            KeyEvent(new CefKeyEvent
            {
                WindowsKeyCode = (int)key,
                EventType = CefKeyEventType.KeyDown,
                Modifiers = Modifiers | GetKeyDirection(key)
            });
        }

        //Keys up
        foreach (WindowsKey key in keyboardEvent.KeysUp)
        {
            KeyEvent(new CefKeyEvent
            {
                WindowsKeyCode = (int)key,
                EventType = CefKeyEventType.KeyUp,
                Modifiers = Modifiers | GetKeyDirection(key)
            });
        }

        //Chars
        foreach (char c in keyboardEvent.Chars)
            KeyEvent(new CefKeyEvent
            {
#if WINDOWS
                WindowsKeyCode = c,
#else
                Character = c,
#endif
                EventType = CefKeyEventType.Char,
                Modifiers = Modifiers
            });
    }

    /// <summary>
    /// State of mouse click events that needs to be persisted for dragging
    /// </summary>
    private CefEventFlags Modifiers = CefEventFlags.None;

    private void UpdateModifiers(MouseClickEvent mouseClickEvent)
    {
        var flag = mouseClickEvent.MouseClickType switch
        {
            MouseClickType.Left => CefEventFlags.LeftMouseButton,
            MouseClickType.Right => CefEventFlags.RightMouseButton,
            MouseClickType.Middle => CefEventFlags.MiddleMouseButton,
            _ => throw new ArgumentException("Click event must be one of 3 states")
        };

        if (mouseClickEvent.MouseEventType == MouseEventType.Up)
        {
            Modifiers &= ~flag;
        }
        else
        {
            Modifiers |= flag;
        }
    }

    private CefEventFlags KeyToFlag(WindowsKey key) => key switch
    {
        // Stateful keys
        WindowsKey.CapsLock => CefEventFlags.CapsLockOn,
        WindowsKey.NumLock => CefEventFlags.NumLockOn,

        WindowsKey.Shift => CefEventFlags.ShiftDown,
        WindowsKey.ShiftKey => CefEventFlags.ShiftDown,
        WindowsKey.LShiftKey => CefEventFlags.ShiftDown,
        WindowsKey.RShiftKey => CefEventFlags.ShiftDown,

        WindowsKey.Control => CefEventFlags.ControlDown,
        WindowsKey.ControlKey => CefEventFlags.ControlDown,
        WindowsKey.LControlKey => CefEventFlags.ControlDown,
        WindowsKey.RControlKey => CefEventFlags.ControlDown,

        WindowsKey.Alt => CefEventFlags.AltGrDown,
        WindowsKey.Menu => CefEventFlags.AltDown,
        WindowsKey.LMenu => CefEventFlags.AltDown,
        WindowsKey.RMenu => CefEventFlags.AltDown,
        // No support for command

        _ => CefEventFlags.None
    };

    private void UpdateModifiers(KeyboardEvent keyboardEvent)
    {
        foreach (var key in keyboardEvent.KeysDown)
        {
            var flag = KeyToFlag(key);

            if ((key is WindowsKey.CapsLock && ((Modifiers & CefEventFlags.CapsLockOn) != CefEventFlags.None))
                || (key is WindowsKey.NumLock && ((Modifiers & CefEventFlags.NumLockOn) != CefEventFlags.None)))
            {
                Modifiers &= ~flag;
            }
            else
            {
                Modifiers |= flag;
            }
        }

        foreach (var key in keyboardEvent.KeysUp)
        {
            var flag = KeyToFlag(key);

            if (key is WindowsKey.CapsLock || key is WindowsKey.NumLock)
            {
                return;
            }

            Modifiers &= ~flag;
        }
    }

    /// <summary>
    ///     Process a <see cref="VoltstroStudios.UnityWebBrowser.Shared.Events.MouseMoveEvent" />
    /// </summary>
    /// <param name="mouseMoveEvent"></param>
    public void ProcessMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
    {
        MouseMoveEvent(new CefMouseEvent
        {
            X = mouseMoveEvent.MouseX,
            Y = mouseMoveEvent.MouseY,
            Modifiers = Modifiers
        });
    }

    /// <summary>
    ///     Process a <see cref="VoltstroStudios.UnityWebBrowser.Shared.Events.MouseClickEvent" />
    /// </summary>
    /// <param name="mouseClickEvent"></param>
    public void ProcessMouseClickEvent(MouseClickEvent mouseClickEvent)
    {
        UpdateModifiers(mouseClickEvent);

        MouseClickEvent(new CefMouseEvent
        {
            X = mouseClickEvent.MouseX,
            Y = mouseClickEvent.MouseY,
            Modifiers = Modifiers
        }, mouseClickEvent.MouseClickCount,
            (CefMouseButtonType)mouseClickEvent.MouseClickType,
            mouseClickEvent.MouseEventType == MouseEventType.Up);
    }

    /// <summary>
    ///     Process a <see cref="VoltstroStudios.UnityWebBrowser.Shared.Events.MouseScrollEvent" />
    /// </summary>
    /// <param name="mouseScrollEvent"></param>
    public void ProcessMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
    {
        MouseScrollEvent(new CefMouseEvent
        {
            X = mouseScrollEvent.MouseX,
            Y = mouseScrollEvent.MouseY,
            Modifiers = Modifiers
        }, mouseScrollEvent.MouseScroll);
    }

    private void KeyEvent(CefKeyEvent keyEvent)
    {
        browserHost.SendKeyEvent(keyEvent);
    }

    private void MouseMoveEvent(CefMouseEvent mouseEvent)
    {
        browserHost.SendMouseMoveEvent(mouseEvent, false);
    }

    private void MouseClickEvent(CefMouseEvent mouseEvent, int clickCount, CefMouseButtonType button, bool mouseUp)
    {
        browserHost.SendMouseClickEvent(mouseEvent, button, mouseUp, clickCount);
    }

    private void MouseScrollEvent(CefMouseEvent mouseEvent, int scroll)
    {
        browserHost.SendMouseWheelEvent(mouseEvent, 0, scroll);
    }

    public void LoadUrl(string url)
    {
        browser.GetMainFrame()?.LoadUrl(url);
    }

    public Vector2 GetMouseScrollPosition()
    {
        return renderHandler.MouseScrollPosition;
    }

    public void LoadHtml(string html)
    {
        browser.GetMainFrame()?.LoadUrl($"data:text/html,{html}");
    }

    public void ExecuteJs(string js)
    {
        browser.GetMainFrame()?.ExecuteJavaScript(js, "", 0);
    }

    public void SetZoomLevel(double zoomLevel)
    {
        browserHost.SetZoomLevel(zoomLevel);
    }

    public double GetZoomLevel()
    {
        return browserHost.GetZoomLevel();
    }

    public void OpenDevTools()
    {
        try
        {
            if (devToolsWindowInfo == null)
            {
                devToolsWindowInfo = CefWindowInfo.Create();
                devToolsClient = new UwbCefPopupClient(proxySettings, () =>
                {
                    devToolsWindowInfo = null;
                    devToolsClient = null;
                    devToolsBrowserSettings = null;
                });
                devToolsBrowserSettings = new CefBrowserSettings();
            }

            browserHost.ShowDevTools(devToolsWindowInfo, devToolsClient, devToolsBrowserSettings, new CefPoint());
        }
        catch (Exception ex)
        {
            mainLogger.LogError(ex, "An error occured while trying to open the dev tools!");
        }
    }

    public void GoBack()
    {
        if (browser.CanGoBack)
            browser.GoBack();
    }

    public void GoForward()
    {
        if (browser.CanGoForward)
            browser.GoForward();
    }

    public void Refresh()
    {
        browser.Reload();
    }

    public void Resize(Resolution resolution)
    {
        renderHandler.Resize(new CefSize((int)resolution.Width, (int)resolution.Height));
        browserHost.WasResized();
    }

    #endregion

    #region Messages

    private readonly Dictionary<string, IMessageBase> messageTypes;

    protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess,
        CefProcessMessage message)
    {
        try
        {
            int index = message.Name.IndexOf(": ", StringComparison.Ordinal);
            if (index == 0)
                return false;

            string messageType = message.Name[..index];
            string messageValue = message.Name[(index + 2)..];

            mainLogger.LogDebug($"Received message of type {messageType}: {messageValue}");

            foreach (KeyValuePair<string, IMessageBase> messageBase in messageTypes)
                if (messageBase.Key == messageType)
                {
                    object value = messageBase.Value.Deserialize(messageValue);
                    messageBase.Value.Execute(value);
                }
        }
        catch (Exception ex)
        {
            mainLogger.LogError(ex, "Error handling message received!");
        }

        return false;
    }

    #endregion
}