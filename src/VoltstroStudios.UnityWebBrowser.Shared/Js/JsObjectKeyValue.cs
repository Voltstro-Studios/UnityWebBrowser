// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Js;

/// <summary>
///     Contains info of an object's key
/// </summary>
internal class JsObjectKeyValue
{
    public JsObjectKeyValue(string key, JsValue value)
    {
        Key = key;
        Value = value;
    }
    
    /// <summary>
    ///     Key
    /// </summary>
    public string Key { get; set; }
    
    /// <summary>
    ///     Value of the key
    /// </summary>
    public JsValue Value { get; set; }
}