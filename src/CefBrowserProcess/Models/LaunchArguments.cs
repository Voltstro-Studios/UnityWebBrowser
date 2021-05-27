using System.IO;
using Xilium.CefGlue;

namespace CefBrowserProcess.Models
{
    public class LaunchArguments
    {
        public string InitialUrl { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool JavaScript { get; set; }
        public byte Bcr { get; set; }
        public byte Bcg { get; set; }
        public byte Bcb { get; set; }
        public byte Bca { get; set; }
        public FileInfo CachePath { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        public int Port { get; set; }
        public bool Debug { get; set; }
        public FileInfo LogPath { get; set; }
        public CefLogSeverity LogSeverity { get; set; }
    }
}