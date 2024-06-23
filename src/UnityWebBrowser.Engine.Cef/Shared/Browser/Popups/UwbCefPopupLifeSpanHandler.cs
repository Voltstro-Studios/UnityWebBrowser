// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using UnityWebBrowser.Engine.Cef.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Popups;

/// <summary>
///     <see cref="CefLifeSpanHandler"/> for a popup
/// </summary>
public class UwbCefPopupLifeSpanHandler : CefLifeSpanHandler
{
    /// <summary>
    ///     Creates a new <see cref="UwbCefPopupLifeSpanHandler"/> instance
    /// </summary>
    /// <param name="onShutdown"></param>
    public UwbCefPopupLifeSpanHandler(Action onShutdown)
    {
        this.onShutdown = onShutdown;
    }

    private readonly Action onShutdown;
    private CefBrowser cefBrowser;

    /// <summary>
    ///     Closes the popup
    /// </summary>
    public void Close()
    {
        cefBrowser?.GetHost().CloseBrowser();
    }

    /// <summary>
    ///     Executes JS in the popup
    /// </summary>
    /// <param name="js"></param>
    public void ExecuteJs(string js)
    {
        cefBrowser?.GetMainFrame()?.ExecuteJavaScript(js, "", 0);
    }
    
    protected override void OnAfterCreated(CefBrowser browser)
    {
        cefBrowser = browser;
    }

    protected override void OnBeforeClose(CefBrowser browser)
    { 
        CefLoggerWrapper.Debug("Closing popup...");
        onShutdown.Invoke();
    }
}