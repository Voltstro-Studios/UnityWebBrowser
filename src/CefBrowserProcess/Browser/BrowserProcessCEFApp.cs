using Xilium.CefGlue;

namespace CefBrowserProcess.Browser
{
    /// <summary>
    ///     <see cref="CefApp"/> for CefBrowserProcess
    /// </summary>
    public class BrowserProcessCEFApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
#if LINUX
            if (!commandLine.HasSwitch("--no-zygote"))
            {
                commandLine.AppendSwitch("--no-zygote");
            }
#endif
        }
    }
}