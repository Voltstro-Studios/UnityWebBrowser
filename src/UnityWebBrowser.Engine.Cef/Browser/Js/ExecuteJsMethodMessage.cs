// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Text.Json.Serialization;
using UnityWebBrowser.Engine.Cef.Browser.Messages;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

namespace UnityWebBrowser.Engine.Cef.Browser.Js;

public class ExecuteJsMethodMessage : MessageBase<ExecuteJsMethodMessage>
{
    public const string ExecuteJsMethodName = "UWBEXECUTEJSMETHOD";
    
    [JsonIgnore]
    public override string MessageName => ExecuteJsMethodName;
    
    [JsonIgnore]
    private readonly ClientControlsActions clientControlsActions;

    public ExecuteJsMethodMessage()
    {
    }
    
    public ExecuteJsMethodMessage(ClientControlsActions clientControlsActions)
    {
        this.clientControlsActions = clientControlsActions;
    }

    public ExecuteJsMethodMessage(string methodName)
    {
        this.MethodName = methodName;
    }

    public override void Execute(ExecuteJsMethodMessage value)
    {
        clientControlsActions.ExecuteJsMethod(value.MethodName);
    }

    #region Message
    
    public string MethodName { get; set; }

    #endregion
}