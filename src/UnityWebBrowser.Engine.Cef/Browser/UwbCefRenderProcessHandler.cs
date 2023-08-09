// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

public class UwbCefRenderProcessHandler : CefRenderProcessHandler
{
    protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
    {
        CefV8Value v8Object = context.GetGlobal();

        CefV8Value versionValue = CefV8Value.CreateString(ThisAssembly.AssemblyFileVersion);
        v8Object.SetValue("uwbEngineVersion", versionValue);
        
        CefV8Value engineName = CefV8Value.CreateString("CEF (Chromium Embedded Framework)");
        v8Object.SetValue("webEngineName", engineName);

        CefV8Value cefVersion = CefV8Value.CreateString(CefRuntime.ChromeVersion);
        v8Object.SetValue("webEngineVersion", cefVersion);
    }
}