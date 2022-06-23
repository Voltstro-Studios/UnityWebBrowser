using System;
using UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Popups;

/// <summary>
///     A <see cref="CefClient"/> that is for popups
/// </summary>
public class UwbCefClientPopup : CefClient
{
    private readonly UwbCefRequestHandler requestHandler;
    private readonly UwbPopupLifeSpanHandler lifeSpanHandler;

    /// <summary>
    ///     Creates a new <see cref="UwbCefClientPopup"/> instance
    /// </summary>
    /// <param name="proxySettings"></param>
    /// <param name="onShutdown"></param>
    public UwbCefClientPopup(ProxySettings proxySettings, Action onShutdown)
    {
        requestHandler = new UwbCefRequestHandler(proxySettings);
        lifeSpanHandler = new UwbPopupLifeSpanHandler(onShutdown);
    }

    /// <summary>
    ///     Closes this <see cref="UwbCefClientPopup"/>
    /// </summary>
    public void Close()
    {
        lifeSpanHandler.Close();
    }

    /// <summary>
    ///     Executes js in this <see cref="UwbCefClientPopup"/>
    /// </summary>
    public void ExecuteJs(string js)
    {
        lifeSpanHandler.ExecuteJs(js);
    }
    
    protected override CefRequestHandler GetRequestHandler()
    {
        return requestHandler;
    }

    protected override CefLifeSpanHandler GetLifeSpanHandler()
    {
        return lifeSpanHandler;
    }
}