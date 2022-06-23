using System;
using UnityWebBrowser.Engine.Cef.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Popups;

public class UwbPopupLifeSpanHandler : CefLifeSpanHandler
{
    public UwbPopupLifeSpanHandler(Action onShutdown)
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
        cefBrowser?.GetHost()?.CloseBrowser();
    }

    /// <summary>
    ///     Executes JS
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