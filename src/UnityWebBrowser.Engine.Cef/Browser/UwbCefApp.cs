using UnityWebBrowser.Engine.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser
{
    /// <summary>
    ///     <see cref="CefApp" /> for CefBrowserProcess
    /// </summary>
    public class UwbCefApp : CefApp
    {
        private readonly bool mediaStreamingEnabled;
        private readonly bool noProxyServer;

        public UwbCefApp(LaunchArguments launchArguments)
        {
            mediaStreamingEnabled = launchArguments.WebRtc;
            noProxyServer = !launchArguments.ProxyEnabled;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            if (noProxyServer && !commandLine.HasSwitch("--no-proxy-server"))
                commandLine.AppendSwitch("--no-proxy-server");

            if (mediaStreamingEnabled && !commandLine.HasSwitch("--enable-media-stream"))
                commandLine.AppendSwitch("--enable-media-stream");

#if LINUX
            if (!commandLine.HasSwitch("--no-zygote"))
            {
                commandLine.AppendSwitch("--no-zygote");
            }
#endif
        }
    }
}