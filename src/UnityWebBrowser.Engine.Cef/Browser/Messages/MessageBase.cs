// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Text.Json;
using System.Text.Json.Serialization;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Messages;

/// <summary>
///     Base class for a message that needs to be transmitted
/// </summary>
public abstract class MessageBase<T> : IMessageBase
{
    [JsonIgnore]
    public abstract string MessageName { get; }

    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, typeof(T), SourceGenerationContext.Default);
    }

    public object Deserialize(string value)
    {
        return (T)JsonSerializer.Deserialize(value, typeof(T), SourceGenerationContext.Default);
    }

    public void Execute(object value)
    {
        Execute((T)value);
    }

    /// <summary>
    ///     Sends the message to the browser process
    /// </summary>
    public void SendMessage()
    {
        CefBrowser browser = CefV8Context.GetCurrentContext().GetBrowser();
        browser!.GetMainFrame()!.SendProcessMessage(CefProcessId.Browser, CefProcessMessage.Create($"{MessageName}: {Serialize(this)}"));
    }

    protected abstract void Execute(T value);
}