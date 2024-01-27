// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Text.Json.Serialization;
using UnityWebBrowser.Engine.Cef.Browser.Js;

namespace UnityWebBrowser.Engine.Cef.Browser.Messages;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(ExecuteJsMethodMessage))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}