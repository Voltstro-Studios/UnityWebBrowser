// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using UnityWebBrowser.Engine.Cef.Shared.Browser.Js;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser;

/// <summary>
///     UWB's custom <see cref="CefRenderProcessHandler" />
///     The render process handles JS callbacks. So custom JS stuff is done here
/// </summary>
public class UwbCefRenderProcessHandler : CefRenderProcessHandler
{
    protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
    {
        CefV8Value v8Object = context.GetGlobal();

        //Root UWB V8 Object
        CefV8Value uwbObject = CefV8Value.CreateObject();

        //Version info
        CefV8Value versionValue = CefV8Value.CreateString(ThisAssembly.AssemblyFileVersion);
        uwbObject.SetValue("EngineVersion", versionValue);

        CefV8Value engineName = CefV8Value.CreateString("CEF (Chromium Embedded Framework)");
        uwbObject.SetValue("EngineName", engineName);

        CefV8Value cefVersion = CefV8Value.CreateString(CefRuntime.ChromeVersion);
        uwbObject.SetValue("CefVersion", cefVersion);

        //UWB's uwbExecuteJsMethod function
        CefV8Value executeMethod = CefV8Value.CreateFunction(UwbCefJsMethodHandler.ExecuteJsMethodsFunctionName,
            new UwbCefJsMethodHandler());
        uwbObject.SetValue(UwbCefJsMethodHandler.ExecuteJsMethodsFunctionName, executeMethod);

        //Add uwb to global value
        v8Object.SetValue("uwb", uwbObject);
    }
}