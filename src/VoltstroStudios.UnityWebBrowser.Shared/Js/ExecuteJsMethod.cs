// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Js;

public struct ExecuteJsMethod
{
    public string MethodName { get; set; }
    
    public JsValue[] Arguments { get; set; }
}