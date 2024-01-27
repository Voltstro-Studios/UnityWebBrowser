// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace UnityWebBrowser.Engine.Cef.Browser.Messages;

public interface IMessageBase
{
    public string MessageName { get; }
    
    public string Serialize(object obj);

    public object Deserialize(string value);
    
    public void Execute(object value);
}