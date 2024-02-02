// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Js;

/// <summary>
///     Holds <see cref="JsObjectKeyValue"/> to be able to build out a type
/// </summary>
public class JsObjectHolder
{
    /// <summary>
    ///     All the keys and values a JS object has
    /// </summary>
    public JsObjectKeyValue[] Keys { get; set; }
}