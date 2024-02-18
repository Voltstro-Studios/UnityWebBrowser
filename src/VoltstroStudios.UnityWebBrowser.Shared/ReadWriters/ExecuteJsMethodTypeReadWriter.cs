// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.UnityWebBrowser.Shared.Js;

namespace VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for <see cref="ExecuteJsMethod"/>
/// </summary>
internal sealed class ExecuteJsMethodTypeReadWriter : TypeReadWriter<ExecuteJsMethod>
{
    public override void Write(BufferedWriter writer, ExecuteJsMethod value)
    {
        //Write Method name first
        writer.WriteString(value.MethodName);
        
        //Write length of arguments
        writer.WriteInt(value.Arguments.Length);
        for (int i = 0; i < value.Arguments.Length; i++)
        {
            JsValue argument = value.Arguments[i];
            WriteJsValue(writer, argument);
        }
    }

    public override ExecuteJsMethod Read(BufferedReader reader)
    {
        string methodName = reader.ReadString();

        int argumentsLength = reader.ReadInt();
        JsValue[] values = new JsValue[argumentsLength];
        for (int i = 0; i < argumentsLength; i++)
        {
            values[i] = ReadJsValue(reader);
        }

        return new ExecuteJsMethod(methodName, values);
    }

    private static void WriteJsValue(BufferedWriter writer, JsValue jsValue)
    {
        //Write type, then use native VoltRpc writer to write the type
        writer.WriteByte((byte)jsValue.Type);
        
        //Tending on type, we will write corresponding type
        switch (jsValue.Type)
        {
            case JsValueType.Object:
                try
                {
                    JsObjectHolder objectHolder = (JsObjectHolder)jsValue.Value;
                    writer.WriteInt(objectHolder.Keys.Length);
                    foreach (JsObjectKeyValue jsObjectValue in objectHolder.Keys)
                    {
                        writer.WriteString(jsObjectValue.Key);
                        WriteJsValue(writer, jsObjectValue.Value);
                    }
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException(
                        $"Failed casting to {typeof(JsObjectHolder).FullName}. Value type was {jsValue.Value.GetType().FullName}.");
                }
                break;
            case JsValueType.Null:
                break;
            case JsValueType.Int:
                writer.WriteInt((int)jsValue.Value);
                break;
            case JsValueType.Bool:
                writer.WriteBool((bool)jsValue.Value);
                break;
            case JsValueType.String:
                writer.WriteString((string)jsValue.Value);
                break;
            case JsValueType.UInt:
                writer.WriteUInt((uint)jsValue.Value);
                break;
            case JsValueType.Double:
                writer.WriteDouble((double)jsValue.Value);
                break;
            case JsValueType.Date:
                DateTime dateTime = (DateTime)jsValue.Value;
                writer.WriteLong(dateTime.Ticks);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static JsValue ReadJsValue(BufferedReader reader)
    {
        JsValueType type = (JsValueType)reader.ReadByte();
        object value = null;
        
        //Depending on what type it is, we will read corresponding type
        switch (type)
        {
            case JsValueType.Null:
                break;
            case JsValueType.Object:
                int objectHolderSize = reader.ReadInt();

                JsObjectKeyValue[] values = new JsObjectKeyValue[objectHolderSize];
                for (int i = 0; i < objectHolderSize; i++)
                {
                    string keyName = reader.ReadString();
                    values[i] = new JsObjectKeyValue(keyName, ReadJsValue(reader));
                }

                value = new JsObjectHolder(values);
                break;
            case JsValueType.Int:
                value = reader.ReadInt();
                break;
            case JsValueType.Bool:
                value = reader.ReadBool();
                break;
            case JsValueType.String:
                value = reader.ReadString();
                break;
            case JsValueType.UInt:
                value = reader.ReadUInt();
                break;
            case JsValueType.Double:
                value = reader.ReadDouble();
                break;
            case JsValueType.Date:
                value = new DateTime(reader.ReadLong(), DateTimeKind.Utc);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return new JsValue(type, value);
    }
}