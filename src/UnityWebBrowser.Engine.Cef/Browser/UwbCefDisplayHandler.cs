// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using UnityWebBrowser.Engine.Cef.Core;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefDisplayHandler" /> implementation
/// </summary>
public class UwbCefDisplayHandler : CefDisplayHandler
{
    private readonly ClientControlsActions clientControls;

    public UwbCefDisplayHandler(UwbCefClient client)
    {
        this.clientControls = client.ClientControls;
    }

    protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
    {
        CefLoggerWrapper.Debug($"{CefLoggerWrapper.FullCefMessageTag} URL change: {url}");
        clientControls.UrlChange(url);
    }

    protected override void OnFullscreenModeChange(CefBrowser browser, bool fullscreen)
    {
        CefLoggerWrapper.Debug($"{CefLoggerWrapper.FullCefMessageTag} Fullscreen change: {fullscreen}");
        clientControls.Fullscreen(fullscreen);
    }

    protected override void OnTitleChange(CefBrowser browser, string title)
    {
        CefLoggerWrapper.Debug($"{CefLoggerWrapper.FullCefMessageTag} Title change: {title}");
        clientControls.TitleChange(title);
    }

    protected override void OnLoadingProgressChange(CefBrowser browser, double progress)
    {
        CefLoggerWrapper.Debug($"{CefLoggerWrapper.FullCefMessageTag} Progress change: {progress}");
        clientControls.ProgressChange(progress);
    }

    protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message,
        string source, int line)
    {
        switch (level)
        {
            case CefLogSeverity.Disable:
                break;
            case CefLogSeverity.Default:
            case CefLogSeverity.Info:
                CefLoggerWrapper.Info($"{CefLoggerWrapper.ConsoleMessageTag} {message}");
                break;
            case CefLogSeverity.Warning:
                CefLoggerWrapper.Warn($"{CefLoggerWrapper.ConsoleMessageTag} {message}");
                break;
            case CefLogSeverity.Error:
            case CefLogSeverity.Fatal:
                CefLoggerWrapper.Error($"{CefLoggerWrapper.ConsoleMessageTag} {message}");
                break;
            case CefLogSeverity.Verbose:
                CefLoggerWrapper.Debug($"{CefLoggerWrapper.ConsoleMessageTag} {message}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        return true;
    }
}