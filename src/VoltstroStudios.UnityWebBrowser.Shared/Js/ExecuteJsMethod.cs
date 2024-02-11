// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Js;

internal struct ExecuteJsMethod
{
    public ExecuteJsMethod(string methodName, JsValue[] arguments)
    {
        MethodName = methodName;
        Arguments = arguments;
    }
    
    public string MethodName { get; set; }
    
    public JsValue[] Arguments { get; set; }
}