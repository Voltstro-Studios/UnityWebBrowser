// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Js;

/// <summary>
///     A value from JS
/// </summary>
internal class JsValue
{
    public JsValue(JsValueType type, object value)
    {
        Type = type;
        Value = value;
    }
    
    /// <summary>
    ///     What <see cref="JsValueType"/> this value is
    /// </summary>
    public JsValueType Type { get; set; }
    
    /// <summary>
    ///     Raw object value
    ///     <para>See <see cref="Type"/> to know what <see cref="System.Type"/> this object is</para>
    /// </summary>
    public object Value { get; set; }
}