// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using Microsoft.Extensions.Logging;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Core;

/// <summary>
///     Wrapper for Cef to use the <see cref="Logger" />
/// </summary>
public static class CefLoggerWrapper
{
    private static ILogger logger;

    internal static void Init(ILogger mainLogger)
    {
        logger = mainLogger;
    }
    
    #region Debug Log

    /// <summary>
    ///     Writes an debug log
    /// </summary>
    /// <param name="message"></param>
    public static void Debug(string message)
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefActionTask.PostTask(CefThreadId.UI, () => Debug(message));
            return;
        }

        logger.LogDebug(message);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Debug(message, values));
            return;
        }

        logger.LogDebug(message, values);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Info(message));
            return;
        }

        logger.LogInformation(message);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Info(message, values));
            return;
        }

        logger.LogInformation(message, values);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Warn(message));
            return;
        }

        logger.LogWarning(message);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Warn(message, values));
            return;
        }

        logger.LogWarning(message, values);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Error(message));
            return;
        }

        logger.LogError(message);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Error(message, values));
            return;
        }

        logger.LogError(message, values);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Error(exception, message));
            return;
        }

        logger.LogError(exception, message);
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
            CefActionTask.PostTask(CefThreadId.UI, () => Error(exception, message, values));
            return;
        }

        logger.LogError(exception, message, values);
    }

    #endregion
}