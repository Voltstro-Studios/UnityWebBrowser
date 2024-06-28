// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using Microsoft.Extensions.Logging;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser;

/// <summary>
///     <see cref="CefDisplayHandler" /> implementation
/// </summary>
internal class UwbCefDisplayHandler : CefDisplayHandler
{
    private readonly ILogger browserConsoleLogger;
    private readonly ClientControlsActions clientControls;
    private readonly ILogger mainLogger;

    public UwbCefDisplayHandler(UwbCefClient client, ILogger mainLogger, ILogger browserConsoleLogger)
    {
        clientControls = client.ClientControls;
        this.mainLogger = mainLogger;
        this.browserConsoleLogger = browserConsoleLogger;
    }

    protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
    {
        mainLogger.LogDebug($"URL change: {url}");
        clientControls.UrlChange(url);
    }

    protected override void OnFullscreenModeChange(CefBrowser browser, bool fullscreen)
    {
        mainLogger.LogDebug($"Fullscreen change: {fullscreen}");
        clientControls.Fullscreen(fullscreen);
    }

    protected override void OnTitleChange(CefBrowser browser, string title)
    {
        mainLogger.LogDebug($"Title change: {title}");
        clientControls.TitleChange(title);
    }

    protected override void OnLoadingProgressChange(CefBrowser browser, double progress)
    {
        mainLogger.LogDebug($"Progress change: {progress}");
        clientControls.ProgressChange(progress);
    }

    protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source,
        int line)
    {
        switch (level)
        {
            case CefLogSeverity.Disable:
                break;
            case CefLogSeverity.Default:
            case CefLogSeverity.Info:
                browserConsoleLogger.LogInformation($"[{source}:{line}]: {message}");
                break;
            case CefLogSeverity.Warning:
                browserConsoleLogger.LogWarning($"[{source}:{line}]: {message}");
                break;
            case CefLogSeverity.Error:
                browserConsoleLogger.LogError($"[{source}:{line}]: {message}");
                break;
            case CefLogSeverity.Fatal:
                browserConsoleLogger.LogCritical($"[{source}:{line}]: {message}");
                break;
            case CefLogSeverity.Verbose:
                //browserConsoleLogger.ZLogDebug($"[{source}:{line}]: {message}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        return true;
    }
}