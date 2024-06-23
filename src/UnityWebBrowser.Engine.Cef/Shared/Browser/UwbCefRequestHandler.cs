// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltstroStudios.UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefRenderHandler" /> implementation
/// </summary>
public class UwbCefRequestHandler : CefRequestHandler
{
    private readonly ProxySettings proxySettings;

    public UwbCefRequestHandler(ProxySettings proxySettings)
    {
        this.proxySettings = proxySettings;
    }

    protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame,
        CefRequest request,
        bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
    {
        return null;
    }

    protected override bool GetAuthCredentials(CefBrowser browser, string originUrl, bool isProxy, string host,
        int port, string realm,
        string scheme, CefAuthCallback callback)
    {
        if (isProxy) callback.Continue(proxySettings.Username, proxySettings.Password);

        return base.GetAuthCredentials(browser, originUrl, isProxy, host, port, realm, scheme, callback);
    }
}