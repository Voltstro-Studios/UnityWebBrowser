using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Engine.Shared.Core.Logging;

public static class Logger
{
    private static Serilog.Core.Logger log;

    private static LoggerConfig loggerConfig;

    private static LoggingLevelSwitch level;

    /// <summary>
    ///     The logger's config, can only be set while the logger isn't running
    /// </summary>
    public static LoggerConfig LoggerConfig
    {
        internal set
        {
            if (IsLoggerInitialized)
                throw new InitializationException("The logger is already initialized!");

            loggerConfig = value;
        }
        get => loggerConfig;
    }

    /// <summary>
    ///     Is the logger initialized?
    ///     <para>Returns true if it is</para>
    /// </summary>
    public static bool IsLoggerInitialized => log != null;

    /// <summary>
    ///     Initializes the logger
    /// </summary>
    /// <exception cref="InitializationException"></exception>
    internal static void Init(LogSeverity logSeverity)
    {
        if (IsLoggerInitialized)
            throw new InitializationException("The logger is already initialized!");

        LoggerConfig ??= new LoggerConfig();

        LogEventLevel logEventLevel;
        switch (logSeverity)
        {
            case LogSeverity.Debug:
                logEventLevel = LogEventLevel.Debug;
                break;
            case LogSeverity.Info:
                logEventLevel = LogEventLevel.Information;
                break;
            case LogSeverity.Warn:
                logEventLevel = LogEventLevel.Warning;
                break;
            case LogSeverity.Error:
                logEventLevel = LogEventLevel.Error;
                break;
            case LogSeverity.Fatal:
                logEventLevel = LogEventLevel.Fatal;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logSeverity), logSeverity, null);
        }

        //Setup logging level
        level = new LoggingLevelSwitch
        {
            MinimumLevel = logEventLevel
        };

        log = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(level)
            .WriteTo.Console(new RenderedCompactJsonFormatter(), logEventLevel)
            .CreateLogger();

        log.Debug("Logger initialized at {Date}", DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));
    }

    /// <summary>
    ///     Shuts down the logger
    /// </summary>
    /// <exception cref="InitializationException"></exception>
    internal static void Shutdown()
    {
        if (!IsLoggerInitialized)
            throw new InitializationException("The logger isn't initialized!");

        log.Debug("Logger shutting down at {Date}", DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"));
        log.Dispose();
        log = null;

        loggerConfig = null;
    }

    private static void CheckInitialization()
    {
        if (!IsLoggerInitialized)
            throw new InitializationException("The logger isn't initialized!");
    }

    #region Debug Logging

    /// <summary>
    ///     Writes a debug log
    /// </summary>
    /// <param name="message"></param>
    public static void Debug(string message)
    {
        CheckInitialization();

        if (level.MinimumLevel == LogEventLevel.Debug)
            log.Debug(message);
    }

    /// <summary>
    ///     Writes a debug log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Debug(string message, params object[] values)
    {
        CheckInitialization();

        if (level.MinimumLevel == LogEventLevel.Debug)
            log.Debug(message, values);
    }

    #endregion

    #region Information Logging

    /// <summary>
    ///     Writes an information log
    /// </summary>
    /// <param name="message"></param>
    public static void Info(string message)
    {
        CheckInitialization();

        log.Information(message);
    }

    /// <summary>
    ///     Writes an information log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Info(string message, params object[] values)
    {
        CheckInitialization();

        log.Information(message, values);
    }

    #endregion

    #region Warning Logging

    /// <summary>
    ///     Writes a warning log
    /// </summary>
    /// <param name="message"></param>
    public static void Warn(string message)
    {
        CheckInitialization();

        log.Warning(message);
    }

    /// <summary>
    ///     Writes a warning log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Warn(string message, params object[] values)
    {
        CheckInitialization();

        log.Warning(message, values);
    }

    #endregion

    #region Error Logging

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="message"></param>
    public static void Error(string message)
    {
        CheckInitialization();

        log.Error(message);
    }

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Error(string message, params object[] values)
    {
        CheckInitialization();

        log.Error(message, values);
    }

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    public static void Error(Exception exception, string message)
    {
        CheckInitialization();

        log.Error(exception, message);
    }

    /// <summary>
    ///     Writes an error log
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="values"></param>
    public static void Error(Exception exception, string message, params object[] values)
    {
        CheckInitialization();

        log.Error(exception, message, values);
    }

    #endregion
}