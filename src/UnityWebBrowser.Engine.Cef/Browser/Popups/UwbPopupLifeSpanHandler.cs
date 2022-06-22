using System;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Popups;

public class UwbPopupLifeSpanHandler : CefLifeSpanHandler
{
    public UwbPopupLifeSpanHandler(Action onShutdown)
    {
        this.onShutdown = onShutdown;
    }

    private readonly Action onShutdown;
    
    protected override void OnBeforeClose(CefBrowser browser)
    {
        onShutdown.Invoke();
    }
}