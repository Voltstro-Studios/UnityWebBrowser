namespace UnityWebBrowser.Engine.Shared.Core.Logging;

/// <summary>
///     The config for <see cref="Logger" />
/// </summary>
public sealed class LoggerConfig
{
    /// <summary>
    ///     The underlying stream will be permit to do buffered writes
    /// </summary>
    public bool BufferedFileWrite = true;

    /// <summary>
    ///     The directory to log files to
    /// </summary>
    public string LogDirectory = "/Logs/";

    /// <summary>
    ///     The format the the files will use
    /// </summary>
    public string LogFileDateTimeFormat = "yyyy-MM-dd-HH-mm-ss";
}