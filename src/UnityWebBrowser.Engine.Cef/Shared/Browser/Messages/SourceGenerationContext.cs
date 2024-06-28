// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Text.Json.Serialization;
using UnityWebBrowser.Engine.Cef.Shared.Browser.Js;
using VoltstroStudios.UnityWebBrowser.Shared.Js;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser.Messages;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(ExecuteJsMethodMessage))]
[JsonSerializable(typeof(JsValue))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(JsObjectHolder))]
[JsonSerializable(typeof(JsObjectKeyValue[]))]
[JsonSerializable(typeof(JsValue[]))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}