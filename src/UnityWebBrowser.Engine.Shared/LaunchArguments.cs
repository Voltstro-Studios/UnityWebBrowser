using System.IO;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Engine.Shared
{
    /// <summary>
    ///     Launch arguments for the app
    /// </summary>
    public class LaunchArguments
    {
        /// <summary>
        ///     The initial URL for the browser
        /// </summary>
        public string InitialUrl { get; init; }

        /// <summary>
        ///     The initial width of the browser
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        ///     The initial height of the browser
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        ///     Is JavaScript enabled
        /// </summary>
        public bool JavaScript { get; init; }

        /// <summary>
        ///     Is Web RTC enabled
        /// </summary>
        public bool WebRtc { get; init; }

        /// <summary>
        ///     The port to use for remote debugging
        /// </summary>
        public int RemoteDebugging { get; init; }

        /// <summary>
        ///     The <see cref="Color"/> to use for the background
        /// </summary>
        public Color BackgroundColor { get; init; }

        /// <summary>
        ///     The path you should use for your cache
        /// </summary>
        public FileInfo CachePath { get; init; }

        /// <summary>
        ///     Should we use a proxy or direct
        /// </summary>
        public bool ProxyEnabled { get; init; }

        /// <summary>
        ///     Username of the proxy
        /// </summary>
        public string ProxyUsername { get; init; }

        /// <summary>
        ///     Password of the proxy
        /// </summary>
        public string ProxyPassword { get; init; }

        /// <summary>
        ///     Pipes or not
        /// </summary>
        public bool Pipes { get; init; }

        /// <summary>
        ///     In location (Either the pipe name or port)
        /// </summary>
        public string InLocation { get; init; }

        /// <summary>
        ///     Out location (Either the pipe name or port)
        /// </summary>
        public string OutLocation { get; init; }

        /// <summary>
        ///     The path you should log browser events to
        /// </summary>
        public FileInfo LogPath { get; init; }

        /// <summary>
        ///     What is the log severity
        /// </summary>
        public LogSeverity LogSeverity { get; init; }
    }
}