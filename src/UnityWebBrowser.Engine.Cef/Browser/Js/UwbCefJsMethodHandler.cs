// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Js;

internal class UwbCefJsMethodHandler : CefV8Handler
{
    public const string UwbCefMessagePrefix = "UWBEXECUTEMETHODJS: ";
    
    protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
    {
        if (name == "uwbExecuteMethod" && arguments.Length > 0)
        {
            //Get name of the function
            CefV8Value nameArgument = arguments[0];
            if (!nameArgument.IsString)
            {
                returnValue = null;
                exception = "Name argument must be typeof string!";
                return true;
            }

            string functionName = nameArgument.GetStringValue();
            
            //Send IPC message to browser process to execute message
            CefBrowser browser = CefV8Context.GetCurrentContext().GetBrowser();
            browser!.GetMainFrame()!.SendProcessMessage(CefProcessId.Browser, CefProcessMessage.Create($"{UwbCefMessagePrefix}{functionName}"));
            
            returnValue = CefV8Value.CreateBool(true);
            exception = null;
            
            return true;
        }
        
        returnValue = null;
        exception = null;
        return false;
    }
}