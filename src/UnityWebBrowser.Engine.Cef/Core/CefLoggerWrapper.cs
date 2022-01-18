using System;
using UnityWebBrowser.Engine.Shared.Core.Logging;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core;

/// <summary>
///     Wrapper for Cef to use the <see cref="Logger"/>
/// </summary>
public static class CefLoggerWrapper
{
    private const string CefMessageTag = "CEF Engine";
    public const string FullCefMessageTag = $"[{CefMessageTag}]:";
    public const string ConsoleMessageTag = $"[{CefMessageTag} Console]:";
    
    #region Debug Log

    /// <summary>
    ///     Writes an debug log
    /// </summary>
    /// <param name="message"></param>
    public static void Debug(string message)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Debug(message));
            return;
        }

        Logger.Debug(message);
    }

    /// <summary>
    ///     Writes an debug log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Debug(string message, params object[] values)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Debug(message, values));
            return;
        }

        Logger.Debug(message, values);
    }

    #endregion

    #region Info Log

    /// <summary>
    ///     Writes an information log
    /// </summary>
    /// <param name="message"></param>
    public static void Info(string message)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Info(message));
            return;
        }

        Logger.Info(message);
    }

    /// <summary>
    ///     Writes an information log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Info(string message, params object[] values)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Info(message, values));
            return;
        }

        Logger.Info(message, values);
    }

    #endregion

    #region Warn

    /// <summary>
    ///     Writes a warning log
    /// </summary>
    /// <param name="message"></param>
    public static void Warn(string message)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Warn(message));
            return;
        }

        Logger.Warn(message);
    }

    /// <summary>
    ///     Writes a warning log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Warn(string message, params object[] values)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Warn(message, values));
            return;
        }

        Logger.Warn(message, values);
    }

    #endregion

    #region Error

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="message"></param>
    public static void Error(string message)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Error(message));
            return;
        }

        Logger.Error(message);
    }

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Error(string message, params object[] values)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Error(message, values));
            return;
        }

        Logger.Error(message, values);
    }

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    public static void Error(Exception exception, string message)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Error(exception, message));
            return;
        }

        Logger.Error(exception, message);
    }

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Error(Exception exception, string message, params object[] values)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, () => Error(exception, message, values));
            return;
        }

        Logger.Error(exception, message, values);
    }

    #endregion
}