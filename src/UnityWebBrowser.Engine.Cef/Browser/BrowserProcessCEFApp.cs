using UnityWebBrowser.Engine.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser
{
    /// <summary>
    ///     <see cref="CefApp"/> for CefBrowserProcess
    /// </summary>
    public class BrowserProcessCEFApp : CefApp
    {
        private readonly bool mediaStreamingEnabled;
        
        public BrowserProcessCEFApp(LaunchArguments launchArguments)
        {
            mediaStreamingEnabled = launchArguments.WebRtc;
        }
        
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            if(mediaStreamingEnabled && !commandLine.HasSwitch("--enable-media-stream"))
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