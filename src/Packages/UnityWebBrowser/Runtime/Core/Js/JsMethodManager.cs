// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VoltstroStudios.UnityWebBrowser.Shared.Js;

namespace VoltstroStudios.UnityWebBrowser.Core.Js
{
    /// <summary>
    ///     The core JS Method Manager.
    ///     <para>JS methods allow the user agent to invoke .NET methods</para>
    /// </summary>
    internal sealed class JsMethodManager
    {
        private readonly Dictionary<Type, JsValueType> typeMatching = new()
        {
            [typeof(int)] = JsValueType.Int,
            [typeof(uint)] = JsValueType.UInt,
            [typeof(bool)] = JsValueType.Bool,
            [typeof(double)] = JsValueType.Double,
            [typeof(string)] = JsValueType.String,
            [typeof(DateTime)] = JsValueType.Date
        };

        internal Dictionary<string, JsMethodInfo> JsMethods { get; } = new();

        /// <summary>
        ///     Registers a method to be able to be invoked by JS
        /// </summary>
        /// <param name="name"></param>
        /// <param name="methodInfo"></param>
        /// <param name="target"></param>
        public void RegisterJsMethod(string name, MethodInfo methodInfo, object target)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            if (JsMethods.ContainsKey(name))
                throw new ArgumentException($"A method of the name {name} is already registered!", nameof(name));
            
            ParameterInfo[] methodParameters = methodInfo.GetParameters();

            CustomPropertyTypeInfo[]? argumentTypes = null;
            if (methodParameters.Length > 0)
            {
                argumentTypes = new CustomPropertyTypeInfo[methodParameters.Length];
                for (int i = 0; i < methodParameters.Length; i++)
                {
                    Type type = methodParameters[i].ParameterType;
                    KeyValuePair<Type, JsValueType> typeMatch = typeMatching.FirstOrDefault(x => x.Key == type);

                    JsValueType valueType = typeMatch.Key == null ? JsValueType.Object : typeMatch.Value;
                    
                    //Have to "process" the type
                    CustomTypeInfo? customTypeInfo = null;
                    if (valueType == JsValueType.Object)
                    {
                        customTypeInfo = CreateCustomTypeInfoForType(type);
                    }
                    
                    argumentTypes[i] = new CustomPropertyTypeInfo
                    {
                        ValueType = valueType,
                        CustomTypeInfo = customTypeInfo
                    };
                }
            }
            
            //method.Method
            JsMethods.Add(name, new JsMethodInfo
            {
                Method = methodInfo,
                Arguments = argumentTypes,
                Target = target
            });
        }
        
        /// <summary>
        ///     Invoke a JS method
        /// </summary>
        /// <param name="executeJsMethod"></param>
        /// <exception cref="InvalidArgumentsException"></exception>
        public void InvokeJsMethod(ExecuteJsMethod executeJsMethod)
        {
            //Get registered method first
            (string? methodName, JsMethodInfo foundMethodInfo) = JsMethods.FirstOrDefault(x => x.Key == executeJsMethod.MethodName);
            if (methodName == null)
                throw new MethodNotFoundException($"Browser tried executing the method '{methodName}', which has not been registered!");

            CustomPropertyTypeInfo[]? foundMethodArguments = foundMethodInfo.Arguments;
            int foundMethodArgumentsLength = foundMethodArguments?.Length ?? 0;
            int passedInMethodArgumentLength = executeJsMethod.Arguments.Length;

            //Make sure arguments count matches first
            if (foundMethodArgumentsLength != passedInMethodArgumentLength)
                throw new InvalidArgumentsException("Passed in arguments count does not match method's arguments count!");

            //Handle arguments
            object[]? arguments = null;
            if (foundMethodArgumentsLength > 0)
            {
                arguments = new object[foundMethodArgumentsLength];
                for (int i = 0; i < foundMethodArgumentsLength; i++)
                {
                    JsValue executedArgument = executeJsMethod.Arguments[i];
                    CustomPropertyTypeInfo matchingArgument = foundMethodArguments![i];

                    if(!(matchingArgument.ValueType == JsValueType.Object && executedArgument.Type == JsValueType.Null)
                       && executedArgument.Type != matchingArgument.ValueType)
                        throw new InvalidArgumentsException($"Invalid argument type! Was excepting '{matchingArgument.ValueType}', but got type of '{executedArgument.Type}'!");

                    object argumentValue = executedArgument.Value;
                    if (matchingArgument.CustomTypeInfo != null && executedArgument.Type == JsValueType.Object)
                    {
                        JsObjectHolder objectHolder = (JsObjectHolder)executedArgument.Value;
                        if(objectHolder.Keys.Length != matchingArgument.CustomTypeInfo.TypeProperties.Length)
                            throw new InvalidArgumentsException($"Passed in argument object keys count does not match what is excepted!");

                        argumentValue = CreateObjectFromObjectHolder(objectHolder, matchingArgument.CustomTypeInfo);
                    }

                    arguments[i] = argumentValue;
                }
            }

            //Invoke method
            foundMethodInfo.Method.Invoke(foundMethodInfo.Target, arguments);
        }

        private static object CreateObjectFromObjectHolder(JsObjectHolder objectHolder, CustomTypeInfo customTypeInfo)
        {
            object argumentValue = Activator.CreateInstance(customTypeInfo.RootType);
            foreach (CustomPropertyTypeInfo customPropertyTypeInfo in customTypeInfo.TypeProperties)
            {
                JsObjectKeyValue? matchedKey = objectHolder.Keys.FirstOrDefault(x => x.Key == customPropertyTypeInfo.PropertyName);
                if (matchedKey == null)
                    throw new InvalidArgumentsException($"Passed in argument object key names does not match what is excepted!");

                if (matchedKey.Value.Type != customPropertyTypeInfo.ValueType)
                    throw new InvalidArgumentsException(
                        "Passed in argument object types does not match what is excepted!");

                object propertyValue = matchedKey.Value.Value;
                if (matchedKey.Value.Type == JsValueType.Object)
                {
                    propertyValue = CreateObjectFromObjectHolder((JsObjectHolder)matchedKey.Value.Value,
                        customPropertyTypeInfo.CustomTypeInfo!);
                }
                            
                customTypeInfo.RootType.GetProperty(customPropertyTypeInfo.PropertyName)!.SetValue(argumentValue, propertyValue);
            }

            return argumentValue;
        }

        private CustomTypeInfo CreateCustomTypeInfoForType(Type type)
        {
            //Get all properties
            PropertyInfo[] properties = type.GetProperties();
            CustomPropertyTypeInfo[] propertyTypeInfos = new CustomPropertyTypeInfo[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                //Find type's matching JsValueType
                PropertyInfo property = properties[i];
                Type propertyType = property.PropertyType;
                KeyValuePair<Type, JsValueType> propertyTypeMatch = typeMatching.FirstOrDefault(x => x.Key == propertyType);
                JsValueType propertyValueType = propertyTypeMatch.Key == null ? JsValueType.Object : propertyTypeMatch.Value;

                //This is an object type, which we need some more info on
                CustomTypeInfo? objectPropertyTypeInfo = null;
                if (propertyValueType == JsValueType.Object)
                    objectPropertyTypeInfo = CreateCustomTypeInfoForType(propertyType);

                propertyTypeInfos[i] = new CustomPropertyTypeInfo
                {
                    ValueType = propertyValueType,
                    CustomTypeInfo = objectPropertyTypeInfo,
                    PropertyName = property.Name
                };
            }

            return new CustomTypeInfo
            {
                RootType = type,
                TypeProperties = propertyTypeInfos
            };
        }
        
        public class CustomPropertyTypeInfo
        {
            public JsValueType ValueType { get; set; }
            
            public string PropertyName { get; set; }
            
            public CustomTypeInfo? CustomTypeInfo { get; set; }
        }
        
        public class CustomTypeInfo
        {
            public Type RootType { get; set; }

            public CustomPropertyTypeInfo[] TypeProperties { get; set; }
        }
        
        public struct JsMethodInfo
        {
            public MethodInfo Method { get; set; }
            public CustomPropertyTypeInfo[]? Arguments { get; set; }
            public object Target { get; set; }
        }
    }
}