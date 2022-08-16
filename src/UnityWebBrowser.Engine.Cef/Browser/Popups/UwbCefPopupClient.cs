using System;
using VoltstroStudios.UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Popups;

/// <summary>
///     A <see cref="CefClient"/> that is for popups
/// </summary>
public class UwbCefPopupClient : CefClient
{
    private readonly UwbCefRequestHandler requestHandler;
    private readonly UwbCefPopupLifeSpanHandler lifeSpanHandler;

    /// <summary>
    ///     Creates a new <see cref="UwbCefPopupClient"/> instance
    /// </summary>
    /// <param name="proxySettings"></param>
    /// <param name="onShutdown"></param>
    public UwbCefPopupClient(ProxySettings proxySettings, Action onShutdown)
    {
        requestHandler = new UwbCefRequestHandler(proxySettings);
        lifeSpanHandler = new UwbCefPopupLifeSpanHandler(onShutdown);
    }

    /// <summary>
    ///     Closes this <see cref="UwbCefPopupClient"/>
    /// </summary>
    public void Close()
    {
        lifeSpanHandler.Close();
    }

    /// <summary>
    ///     Executes js in this <see cref="UwbCefPopupClient"/>
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