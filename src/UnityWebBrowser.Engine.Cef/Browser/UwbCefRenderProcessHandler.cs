using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

public class UwbCefRenderProcessHandler : CefRenderProcessHandler
{
    protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
    {
        CefV8Value v8Object = context.GetGlobal();

        CefV8Value versionValue = CefV8Value.CreateString(ThisAssembly.Info.Version);
        v8Object.SetValue("uwbVersion", versionValue);
        
        CefV8Value cefVersion = CefV8Value.CreateString(CefRuntime.ChromeVersion);
        v8Object.SetValue("cefVersion", cefVersion);
    }
}