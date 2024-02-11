// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Js;

/// <summary>
///     Main details on a method that JS wants to execute
/// </summary>
internal struct ExecuteJsMethod
{
    public ExecuteJsMethod(string methodName, JsValue[] arguments)
    {
        MethodName = methodName;
        Arguments = arguments;
    }
    
    /// <summary>
    ///     The name of the method
    /// </summary>
    public string MethodName { get; set; }
    
    /// <summary>
    ///     All arguments of the method
    /// </summary>
    public JsValue[] Arguments { get; set; }
}