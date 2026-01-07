// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using UnityWebBrowser.Engine.Cef.Core;
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
    private readonly bool ignoreSslErrors;
    private readonly string[] ignoreSslErrorsDomains;
    private CefWindowInfo devToolsWindowInfo;
    
    //State of mouse click events that needs to be persisted for dragging
    private CefEventFlags modifiers = CefEventFlags.None;

    /// <summary>
    ///     Creates a new <see cref="UwbCefClient" /> instance
    /// </summary>
    public UwbCefClient(
        CefSize size,
        PopupAction popupAction,
        EnginePopupManager popupManager,
        ProxySettings proxySettings,
        bool ignoreSslErrors,
        string[] ignoreSslErrorsDomains,
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
        lifespanHandler = new UwbCefLifespanHandler(popupAction, popupManager, proxySettings, ignoreSslErrors, ignoreSslErrorsDomains);
        lifespanHandler.AfterCreated += cefBrowser =>
        {
            browser = cefBrowser;
            browserHost = cefBrowser.GetHost();
            ClientControls.Ready();
        };
        displayHandler = new UwbCefDisplayHandler(this, mainLogger, browserConsoleLogger);
        requestHandler = new UwbCefRequestHandler(proxySettings, ignoreSslErrors, ignoreSslErrorsDomains);
        contextMenuHandler = new UwbCefContextMenuHandler();

        this.ignoreSslErrors = ignoreSslErrors;
        this.ignoreSslErrorsDomains = ignoreSslErrorsDomains;

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

    /// <summary>
    ///     Process a <see cref="KeyboardEvent" />
    /// </summary>
    /// <param name="keyboardEvent"></param>
    public void ProcessKeyboardEvent(KeyboardEvent keyboardEvent)
    {
        UpdateModifiersWithKeyboard(keyboardEvent);

        //Keys down
        foreach (WindowsKey key in keyboardEvent.KeysDown)
        {
            CefKeyEvent keyEvent = new()
            {
                WindowsKeyCode = (int)key,
                EventType = CefKeyEventType.KeyDown,
                Modifiers = modifiers | UwbCefClientUtils.GetKeyDirection(key)
            };
            
            browserHost.SendKeyEvent(keyEvent);
        }

        //Keys up
        foreach (WindowsKey key in keyboardEvent.KeysUp)
        {
            CefKeyEvent keyEvent = new()
            {
                WindowsKeyCode = (int)key,
                EventType = CefKeyEventType.KeyUp,
                Modifiers = modifiers | UwbCefClientUtils.GetKeyDirection(key)
            };
            
            browserHost.SendKeyEvent(keyEvent);
        }

        //Chars
        foreach (char c in keyboardEvent.Chars)
        {
            CefKeyEvent keyEvent = new()
            {
                WindowsKeyCode = c,
                Character = c,
                EventType = CefKeyEventType.Char,
                Modifiers = modifiers
            };
            
            browserHost.SendKeyEvent(keyEvent);
        }
    }

    private void UpdateModifiersWithMouse(MouseClickEvent mouseClickEvent)
    {
        CefEventFlags flag = mouseClickEvent.MouseClickType switch
        {
            MouseClickType.Left => CefEventFlags.LeftMouseButton,
            MouseClickType.Right => CefEventFlags.RightMouseButton,
            MouseClickType.Middle => CefEventFlags.MiddleMouseButton,
            _ => throw new ArgumentException("Click event must be one of 3 states")
        };

        if (mouseClickEvent.MouseEventType == MouseEventType.Up)
            modifiers &= ~flag;
        else
            modifiers |= flag;
    }

    private void UpdateModifiersWithKeyboard(KeyboardEvent keyboardEvent)
    {
        foreach (WindowsKey key in keyboardEvent.KeysDown)
        {
            CefEventFlags flag = UwbCefClientUtils.KeyToFlag(key);

            if ((key is WindowsKey.CapsLock && (modifiers & CefEventFlags.CapsLockOn) != CefEventFlags.None)
                || (key is WindowsKey.NumLock && (modifiers & CefEventFlags.NumLockOn) != CefEventFlags.None))
                modifiers &= ~flag;
            else
                modifiers |= flag;
        }

        foreach (WindowsKey key in keyboardEvent.KeysUp)
        {
            CefEventFlags flag = UwbCefClientUtils.KeyToFlag(key);

            if (key is WindowsKey.CapsLock or WindowsKey.NumLock)
                return;

            modifiers &= ~flag;
        }
    }

    /// <summary>
    ///     Process a <see cref="VoltstroStudios.UnityWebBrowser.Shared.Events.MouseMoveEvent" />
    /// </summary>
    /// <param name="mouseMoveEvent"></param>
    public void ProcessMouseMoveEvent(MouseMoveEvent mouseMoveEvent)
    {
        CefMouseEvent mouseEvent = new CefMouseEvent
        {
            X = mouseMoveEvent.MouseX,
            Y = mouseMoveEvent.MouseY,
            Modifiers = modifiers
        };
        
        browserHost.SendMouseMoveEvent(mouseEvent, false);
    }

    /// <summary>
    ///     Process a <see cref="VoltstroStudios.UnityWebBrowser.Shared.Events.MouseClickEvent" />
    /// </summary>
    /// <param name="mouseClickEvent"></param>
    public void ProcessMouseClickEvent(MouseClickEvent mouseClickEvent)
    {
        UpdateModifiersWithMouse(mouseClickEvent);

        CefMouseEvent mouseEvent = new()
        {
            X = mouseClickEvent.MouseX,
            Y = mouseClickEvent.MouseY,
            Modifiers = modifiers
        };
        
        browserHost.SendMouseClickEvent(
            mouseEvent,
            (CefMouseButtonType)mouseClickEvent.MouseClickType,
            mouseClickEvent.MouseEventType == MouseEventType.Up,
            mouseClickEvent.MouseClickCount);
    }

    /// <summary>
    ///     Process a <see cref="VoltstroStudios.UnityWebBrowser.Shared.Events.MouseScrollEvent" />
    /// </summary>
    /// <param name="mouseScrollEvent"></param>
    public void ProcessMouseScrollEvent(MouseScrollEvent mouseScrollEvent)
    {
        CefMouseEvent mouseEvent = new()
        {
            X = mouseScrollEvent.MouseX,
            Y = mouseScrollEvent.MouseY,
            Modifiers = modifiers
        };
        
        browserHost.SendMouseWheelEvent(mouseEvent, 0, mouseScrollEvent.MouseScroll);
    }
    
    /// <summary>
    ///     Load a URL
    /// </summary>
    /// <param name="url"></param>
    public void LoadUrl(string url)
    {
        browser.GetMainFrame()!.LoadUrl(url);
    }

    /// <summary>
    ///     Gets current mouse scroll position
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMouseScrollPosition()
    {
        return renderHandler.MouseScrollPosition;
    }

    /// <summary>
    ///     Loads HTML content
    /// </summary>
    /// <param name="html"></param>
    public void LoadHtml(string html)
    {
        html = CefRuntime.Base64Encode(Encoding.UTF8.GetBytes(html));
        html = CefRuntime.UriEncode(html, false);
        
        browser.GetMainFrame()!.LoadUrl($"data:text/html;base64,{html}");
    }

    /// <summary>
    ///     Executes JS
    /// </summary>
    /// <param name="js"></param>
    public void ExecuteJs(string js)
    {
        browser.GetMainFrame()!.ExecuteJavaScript(js, "", 0);
    }

    /// <summary>
    ///     Sets a zoom level
    /// </summary>
    /// <param name="zoomLevel"></param>
    public void SetZoomLevel(double zoomLevel)
    {
        browserHost.SetZoomLevel(zoomLevel);
    }

    /// <summary>
    ///     Gets current zoom level
    /// </summary>
    /// <returns></returns>
    public double GetZoomLevel()
    {
        return browserHost.GetZoomLevel();
    }

    /// <summary>
    ///     Opens DevTools window
    /// </summary>
    public void OpenDevTools()
    {
        try
        {
            if (devToolsWindowInfo == null)
            {
                devToolsWindowInfo = CefWindowInfo.Create();
                devToolsClient = new UwbCefPopupClient(
                    proxySettings, () =>
                    {
                        devToolsWindowInfo = null;
                        devToolsClient = null;
                        devToolsBrowserSettings = null;
                    },
                    ignoreSslErrors,
                    ignoreSslErrorsDomains);
                devToolsBrowserSettings = new CefBrowserSettings();
            }

            browserHost.ShowDevTools(devToolsWindowInfo, devToolsClient, devToolsBrowserSettings, new CefPoint());
        }
        catch (Exception ex)
        {
            mainLogger.LogError(ex, "An error occured while trying to open the dev tools!");
        }
    }

    /// <summary>
    ///     Goes back a page
    /// </summary>
    public void GoBack()
    {
        if (browser.CanGoBack)
            browser.GoBack();
    }

    /// <summary>
    ///     Goes forward a page
    /// </summary>
    public void GoForward()
    {
        if (browser.CanGoForward)
            browser.GoForward();
    }

    /// <summary>
    ///     Refreshes current page
    /// </summary>
    public void Refresh()
    {
        browser.Reload();
    }

    /// <summary>
    ///     Resizes render window to a new resolution
    /// </summary>
    /// <param name="resolution"></param>
    public void Resize(Resolution resolution)
    {
        //HACK: Workaround to force cef to re-render since browser is not correctly refreshing on resize
        //https://github.com/chromiumembedded/cef/issues/3826
        browserHost.WasHidden(true);
        
        //Do actual resize
        renderHandler.Resize(new CefSize((int)resolution.Width, (int)resolution.Height));
        browserHost.WasResized();
        
        browserHost.WasHidden(false);
    }

    public void AudioMute(bool muted)
    {
        browserHost.SetAudioMuted(muted);
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