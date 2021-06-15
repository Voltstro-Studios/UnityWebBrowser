using System.IO;
using UnityWebBrowser.Shared;

namespace UnityWebBrowser.Engine.Shared
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <summary>
    ///     Launch arguments for the app
    /// </summary>
    public class LaunchArguments
    {
        /// <summary>
        ///     The initial URL for the browser
        /// </summary>
        public string InitialUrl { get; set; }
        
        /// <summary>
        ///     The initial width of the browser
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        ///     The initial height of the browser
        /// </summary>
        public int Height { get; set; }
        
        /// <summary>
        ///     Is JavaScript enabled
        /// </summary>
        public bool JavaScript { get; set; }
        
        /// <summary>
        ///     Is Web RTC enabled
        /// </summary>
        public bool WebRtc { get; set; }
        
        /// <summary>
        ///     The port to use for remote debugging
        /// </summary>
        public int RemoteDebugging { get; set; }
        
        /// <summary>
        ///     Background color (red)
        /// </summary>
        public byte Bcr { get; set; }
        
        /// <summary>
        ///     Background color (green)
        /// </summary>
        public byte Bcg { get; set; }
        
        /// <summary>
        ///     Background color (blue)
        /// </summary>
        public byte Bcb { get; set; }
        
        /// <summary>
        ///     Background color (alpha)
        /// </summary>
        public byte Bca { get; set; }
        
        /// <summary>
        ///     The path you should use for your cache
        /// </summary>
        public FileInfo CachePath { get; set; }
        
        /// <summary>
        ///     Should we use a proxy or direct
        /// </summary>
        public bool ProxyEnabled { get; set; }
        
        /// <summary>
        ///     Username of the proxy
        /// </summary>
        public string ProxyUsername { get; set; }
        
        /// <summary>
        ///     Password of the proxy
        /// </summary>
        public string ProxyPassword { get; set; }
        
        /// <summary>
        ///     Port for IPC
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     The path you should log browser events to
        /// </summary>
        public FileInfo LogPath { get; set; }
        
        /// <summary>
        ///     What is the log severity
        /// </summary>
        public LogSeverity LogSeverity { get; set; }
    }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
}