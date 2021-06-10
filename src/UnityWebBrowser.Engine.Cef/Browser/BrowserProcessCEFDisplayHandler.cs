using System;
using UnityWebBrowser.Engine.Cef.Core;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser
{
    /// <summary>
    ///     <see cref="CefDisplayHandler"/> implementation
    /// </summary>
    public class BrowserProcessCEFDisplayHandler : CefDisplayHandler
    {
        protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            switch (level)
            {
                case CefLogSeverity.Disable:
                    break;
                case CefLogSeverity.Default:
                case CefLogSeverity.Info:
                    Logger.Info($"CEF: {message}");
                    break;
                case CefLogSeverity.Warning:
                    Logger.Warn($"CEF: {message}");
                    break;
                case CefLogSeverity.Error:
                case CefLogSeverity.Fatal:
                    Logger.Error($"CEF: {message}");
                    break;
                case CefLogSeverity.Verbose:
                    Logger.Debug($"CEF: {message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            return true;
        }
    }
}