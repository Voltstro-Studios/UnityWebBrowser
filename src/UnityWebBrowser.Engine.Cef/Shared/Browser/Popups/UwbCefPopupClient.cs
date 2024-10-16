// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltstroStudios.UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser.Popups;

/// <summary>
///     A <see cref="CefClient" /> that is for popups
/// </summary>
public class UwbCefPopupClient : CefClient
{
    private readonly UwbCefPopupLifeSpanHandler lifeSpanHandler;
    private readonly UwbCefRequestHandler requestHandler;

    /// <summary>
    ///     Creates a new <see cref="UwbCefPopupClient" /> instance
    /// </summary>
    /// <param name="proxySettings"></param>
    /// <param name="onShutdown"></param>
    /// <param name="ignoreSslErrors"></param>
    /// <param name="ignoreSslErrorsDomains"></param>
    public UwbCefPopupClient(
        ProxySettings proxySettings,
        Action onShutdown,
        bool ignoreSslErrors,
        string[] ignoreSslErrorsDomains)
    {
        requestHandler = new UwbCefRequestHandler(proxySettings, ignoreSslErrors, ignoreSslErrorsDomains);
        lifeSpanHandler = new UwbCefPopupLifeSpanHandler(onShutdown);
    }

    /// <summary>
    ///     Closes this <see cref="UwbCefPopupClient" />
    /// </summary>
    public void Close()
    {
        lifeSpanHandler.Close();
    }

    /// <summary>
    ///     Executes js in this <see cref="UwbCefPopupClient" />
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