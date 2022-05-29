using UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     A <see cref="CefClient"/> that is for popups
/// </summary>
public class UwbCefClientPopup : CefClient
{
    private readonly UwbCefRequestHandler requestHandler;
    
    /// <summary>
    ///     Creates a new <see cref="UwbCefClientPopup"/> instance
    /// </summary>
    /// <param name="proxySettings"></param>
    public UwbCefClientPopup(ProxySettings proxySettings)
    {
        requestHandler = new UwbCefRequestHandler(proxySettings);
    }
    
    protected override CefRequestHandler GetRequestHandler()
    {
        return requestHandler;
    }
}