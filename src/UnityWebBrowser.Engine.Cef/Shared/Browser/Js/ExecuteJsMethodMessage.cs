// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityWebBrowser.Engine.Cef.Shared.Browser.Messages;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Shared.Js;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser.Js;

internal class ExecuteJsMethodMessage : MessageBase<ExecuteJsMethodMessage>
{
    public const string ExecuteJsMethodName = "UWBEXECUTEJSMETHOD";

    [JsonIgnore] private readonly ClientControlsActions clientControlsActions;

    public ExecuteJsMethodMessage()
    {
    }

    public ExecuteJsMethodMessage(ClientControlsActions clientControlsActions)
    {
        this.clientControlsActions = clientControlsActions;
    }

    public ExecuteJsMethodMessage(string methodName, JsValue[] arguments)
    {
        MethodName = methodName;
        Arguments = arguments;
    }

    [JsonIgnore] public override string MessageName => ExecuteJsMethodName;

    protected override void Execute(ExecuteJsMethodMessage value)
    {
        ExecuteJsMethod executeJsMethod = new(value.MethodName, value.Arguments);

        //Since JSValue.Value is an object, System.Text.Json will deserialize as a JsonElement
        //Pain in the ass, but we can fix it
        for (int i = 0; i < value.Arguments.Length; i++)
        {
            JsValue argument = value.Arguments[i];
            if (argument.Value is JsonElement element)
            {
                JsonElementToJsValue(ref argument, element);
                value.Arguments[i] = argument;
            }
        }

        clientControlsActions.ExecuteJsMethod(executeJsMethod);
    }

    /// <summary>
    ///     Converts a <see cref="JsonElement" /> to a <see cref="JsValue" />
    /// </summary>
    /// <param name="jsValue"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static void JsonElementToJsValue(ref JsValue jsValue, JsonElement element)
    {
        switch (jsValue.Type)
        {
            case JsValueType.Null:
                break;
            case JsValueType.Object:
                //Objects use type JsObjectHolder
                JsObjectHolder objectHolder =
                    (JsObjectHolder)element.Deserialize(typeof(JsObjectHolder), SourceGenerationContext.Default)!;
                foreach (JsObjectKeyValue jsKey in objectHolder.Keys)
                {
                    JsValue value = jsKey.Value;
                    if (value.Value is JsonElement objectElement)
                    {
                        JsonElementToJsValue(ref value, objectElement);
                        jsKey.Value = value;
                    }
                }

                jsValue.Value = objectHolder;
                break;
            case JsValueType.Bool:
                jsValue.Value = element.GetBoolean();
                break;
            case JsValueType.Int:
                jsValue.Value = element.GetInt32();
                break;
            case JsValueType.UInt:
                jsValue.Value = element.GetUInt32();
                break;
            case JsValueType.Double:
                jsValue.Value = element.GetDouble();
                break;
            case JsValueType.Date:
                jsValue.Value = element.GetDateTime();
                break;
            case JsValueType.String:
                jsValue.Value = element.GetString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region Message

    /// <summary>
    ///     Name of the method to execute
    /// </summary>
    public string MethodName { get; set; }

    /// <summary>
    ///     All provided arguments
    /// </summary>
    public JsValue[] Arguments { get; set; }

    #endregion
}