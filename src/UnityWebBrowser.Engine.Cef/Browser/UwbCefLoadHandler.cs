// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Linq;
using UnityWebBrowser.Engine.Cef.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefLoadHandler" /> implementation
/// </summary>
public class UwbCefLoadHandler : CefLoadHandler
{
    private readonly UwbCefClient client;
    private readonly string[] ignoredLoadUrls = {"about:blank"};

    internal UwbCefLoadHandler(UwbCefClient client)
    {
        this.client = client;
    }

    protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
    {
        string url = frame.Url;
        if (ignoredLoadUrls.Contains(url))
            return;

        CefLoggerWrapper.Debug($"{CefLoggerWrapper.FullCefMessageTag} Load start: {url}");

        if (frame.IsMain) client.ClientControls.LoadStart(url);
    }

    protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
    {
        string url = frame.Url;
        if (ignoredLoadUrls.Contains(url))
            return;

        CefLoggerWrapper.Debug($"{CefLoggerWrapper.FullCefMessageTag} Load end: {url}");

        if (frame.IsMain) client.ClientControls.LoadFinish(url);
    }

    protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode,
        string errorText, string failedUrl)
    {
        CefLoggerWrapper.Error(
            $"{CefLoggerWrapper.ConsoleMessageTag} An error occurred while trying to load '{failedUrl}'! Error details: {errorText} (Code: {errorCode})");

        if (errorCode is CefErrorCode.Aborted
            or CefErrorCode.BLOCKED_BY_RESPONSE
            or CefErrorCode.BLOCKED_BY_CLIENT
            or CefErrorCode.BLOCKED_BY_CSP)
            return;

        //TODO: We should move this to an internal scheme page thingy
        string html =
            $@"<style>
@import url('https://fonts.googleapis.com/css2?family=Ubuntu&display=swap');
body {{
font-family: 'Ubuntu', sans-serif;
}}
</style>
<h2>An error occurred while trying to load '{failedUrl}'!</h2>
<p>Error: {errorText}<br>(Code: {(int) errorCode})</p>";
        client.LoadHtml(html);
    }
}