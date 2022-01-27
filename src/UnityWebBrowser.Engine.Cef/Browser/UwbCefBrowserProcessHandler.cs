using UnityWebBrowser.Engine.Cef.Browser.Schemes;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

public class UwbCefBrowserProcessHandler : CefBrowserProcessHandler
{
    protected override void OnContextInitialized()
    {
        CefRuntime.RegisterSchemeHandlerFactory("uwb", "about", new AboutSchemeHandler());
    }
}