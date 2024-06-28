// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using VoltstroStudios.UnityWebBrowser.Shared;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     Manager for creating <see cref="ILogger{TCategoryName}"/>s
/// </summary>
public sealed class LoggerManager
{
    private readonly ILoggerFactory loggerFactory;
    
    public LoggerManager(LogSeverity logSeverity)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateLogger();
        
        loggerFactory = LoggerFactory.Create(logging =>
        {
            LogLevel level = logSeverity switch
            {
                LogSeverity.Debug => LogLevel.Debug,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Warn => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Fatal => LogLevel.Critical,
                _ => throw new ArgumentOutOfRangeException(nameof(logSeverity), logSeverity, null)
            };

            logging.SetMinimumLevel(level);
            logging.AddSerilog(dispose: true);
        });
    }

    /// <summary>
    ///     Creates a new <see cref="ILogger{TCategoryName}"/> instance
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string name)
    {
        return loggerFactory.CreateLogger(name);
    }
}