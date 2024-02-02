// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltstroStudios.UnityWebBrowser.Shared.Js;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser.Js;

/// <summary>
///     <see cref="CefV8Handler"/> for uwbExecuteMethod
/// </summary>
internal class UwbCefJsMethodHandler : CefV8Handler
{
    public const string ExecuteJsMethodsFunctionName = "ExecuteJsMethod";
    
    protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
    {
        if (name == ExecuteJsMethodsFunctionName && arguments.Length > 0)
        {
            //Get name of the function
            CefV8Value nameArgument = arguments[0];
            if (!nameArgument.IsString)
            {
                returnValue = null;
                exception = "Name argument must be typeof string!";
                return true;
            }

            try
            {
                string functionName = nameArgument.GetStringValue();

                //Process all other arguments
                JsValue[] jsValues = new JsValue[arguments.Length - 1];
                for (int i = 1; i < arguments.Length; i++)
                {
                    CefV8Value argument = arguments[i];
                    jsValues[i - 1] = ReadCefValueToJsValue(argument);
                }
                
                //Create message
                ExecuteJsMethodMessage message = new(functionName, jsValues);
                message.SendMessage();

                returnValue = CefV8Value.CreateBool(true);
                exception = null;

                return true;
            }
            catch (Exception ex)
            {
                returnValue = null;
                exception =
                    $"An internal error occured while executing uwbExecuteMethod! {ex.Message}\n{ex.StackTrace}";
                return true;
            }
        }
        
        returnValue = null;
        exception = null;
        return false;
    }

    private static JsValue ReadCefValueToJsValue(CefV8Value cefValue)
    {
        object value = null;
        JsValueType type = JsValueType.Null;
        
        if (cefValue.IsPromise || cefValue.IsFunction || cefValue.IsArrayBuffer || cefValue.IsArray)
        {
            //returnValue = null;
            //exception = "Argument cannot be a promise or function!";
            //return true;
            //TODO: Custom exception to filter
            throw new Exception("Argument cannot be a promise, function, an array or an array buffer!");
        }
        
        if (cefValue.IsNull || cefValue.IsUndefined)
        {
            value = null;
            type = JsValueType.Null;
        }
        else if (cefValue.IsObject)
        {
            //Custom object
            string[] keys = cefValue.GetKeys();
            JsObjectKeyValue[] values = new JsObjectKeyValue[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                values[i] = new JsObjectKeyValue
                {
                    Key = key,
                    Value = ReadCefValueToJsValue(cefValue.GetValue(key))
                };
            }
            
            //Custom objects are held by a JsObjectHolder
            JsObjectHolder objectHolder = new()
            {
                Keys = values
            };

            value = objectHolder;
            type = JsValueType.Object;
        }
        else if (cefValue.IsBool)
        {
            value = cefValue.GetBoolValue();
            type = JsValueType.Bool;
        }
        else if (cefValue.IsInt)
        {
            value = cefValue.GetIntValue();
            type = JsValueType.Int;
        }
        else if (cefValue.IsUInt)
        {
            value = cefValue.GetUIntValue();
            type = JsValueType.UInt;
        }
        else if (cefValue.IsDouble)
        {
            value = cefValue.GetDoubleValue();
            type = JsValueType.Double;
        }
        else if (cefValue.IsString)
        {
            value = cefValue.GetStringValue();
            type = JsValueType.String;
        }
        else if (cefValue.IsDate)
        {
            //TODO: Fix
            CefBaseTime time = cefValue.GetDateValue();
            DateTime dateTime = DateTime.FromFileTime(time.Ticks);

            value = dateTime;
            type = JsValueType.Date;
        }
        
        return new JsValue
        {
            Value = value,
            Type = type
        };
    }
}