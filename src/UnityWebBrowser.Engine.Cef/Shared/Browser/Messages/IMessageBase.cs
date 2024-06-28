// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace UnityWebBrowser.Engine.Cef.Shared.Browser.Messages;

/// <summary>
///     Base interface for <see cref="MessageBase{T}" />
/// </summary>
public interface IMessageBase
{
    public object Deserialize(string value);

    public void Execute(object value);
}