// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Linq;
using UnityWebBrowser.Engine.Cef.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser;

/// <summary>
///     <see cref="CefLoadHandler" /> implementation
/// </summary>
public class UwbCefLoadHandler : CefLoadHandler
{
    private readonly UwbCefClient client;
    private readonly string[] ignoredLoadUrls = { "about:blank" };

    internal UwbCefLoadHandler(UwbCefClient client)
    {
        this.client = client;
    }

    protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
    {
        string url = frame.Url;
        if (ignoredLoadUrls.Contains(url))
            return;

        CefLoggerWrapper.Debug($"Load start: {url}");

        if (frame.IsMain) client.ClientControls.LoadStart(url);
    }

    protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
    {
        string url = frame.Url;
        if (ignoredLoadUrls.Contains(url))
            return;

        CefLoggerWrapper.Debug($"Load end: {url}");

        if (frame.IsMain) client.ClientControls.LoadFinish(url);
    }
}