// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Shared.Js;

/// <summary>
///     What data type a JS value is
/// </summary>
public enum JsValueType : byte
{
    /// <summary>
    ///     Null/Undefined
    /// </summary>
    Null,
    
    /// <summary>
    ///     <see cref="bool"/>
    /// </summary>
    Bool,
    
    /// <summary>
    ///     <see cref="int"/>
    /// </summary>
    Int,
    
    /// <summary>
    ///     <see cref="uint"/>
    /// </summary>
    UInt,
    
    /// <summary>
    ///     <see cref="double"/>
    /// </summary>
    Double,
    
    /// <summary>
    ///     <see cref="DateTime"/>
    /// </summary>
    Date,
    
    /// <summary>
    ///     <see cref="string"/>
    /// </summary>
    String,
    
    /// <summary>
    ///     <see cref="object"/>
    ///     <para><see cref="JsValue.Value"/> will be of type <see cref="JsObjectHolder"/></para>
    /// </summary>
    Object
}