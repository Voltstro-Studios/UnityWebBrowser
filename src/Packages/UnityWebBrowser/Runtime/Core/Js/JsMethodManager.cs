// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using VoltstroStudios.UnityWebBrowser.Shared.Js;

namespace VoltstroStudios.UnityWebBrowser.Core.Js
{
    /// <summary>
    ///     The core JS Method Manager.
    ///     <para>JS methods allow the user agent to invoke .NET methods</para>
    /// </summary>
    [Serializable]
    [Preserve]
    public sealed class JsMethodManager
    {
        /// <summary>
        ///     Enables/Disables JS Methods
        /// </summary>
        [Tooltip("Enables/Disables JS Methods")]
        public bool jsMethodsEnable;
        
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
        /// <param name="name">Name of the method</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> of the method</param>
        /// <param name="target">Target <see cref="object"/> that the method lives on</param>
        /// <exception cref="NotEnabledException">Thrown if <see cref="jsMethodsEnable"/> is false</exception>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="name"/>, <see cref="methodInfo"/> or <see cref="target"/> are null or empty</exception>
        /// <exception cref="ArgumentException">Thrown if the name has already been used</exception>
        /// <exception cref="UnsupportedTypeException">Thrown if the method returns anything other then void</exception>
        public void RegisterJsMethod(string name, MethodInfo methodInfo, object target)
        {
            if (!jsMethodsEnable)
                throw new NotEnabledException(
                    $"The JS Method manager is disabled! You need to enable it using {nameof(jsMethodsEnable)}.");
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            if (JsMethods.ContainsKey(name))
                throw new ArgumentException($"A method of the name {name} is already registered!", nameof(name));

            if (methodInfo.ReturnType != typeof(void))
                throw new UnsupportedTypeException("The provided method must return void!");
            
            ParameterInfo[] methodParameters = methodInfo.GetParameters();

            MethodArgument[]? argumentTypes = null;
            if (methodParameters.Length > 0)
            {
                argumentTypes = new MethodArgument[methodParameters.Length];
                for (int i = 0; i < methodParameters.Length; i++)
                {
                    Type type = methodParameters[i].ParameterType;
                    if (type.IsArray)
                        throw new UnsupportedTypeException("Parameters cannot be an array type!");
                    
                    KeyValuePair<Type, JsValueType> typeMatch = typeMatching.FirstOrDefault(x => x.Key == type);

                    JsValueType valueType = typeMatch.Key == null ? JsValueType.Object : typeMatch.Value;
                    
                    //Have to "process" the type
                    CustomTypeInfo? customTypeInfo = null;
                    if (valueType == JsValueType.Object)
                    {
                        customTypeInfo = CreateCustomTypeInfoForType(type);
                    }

                    argumentTypes[i] = new MethodArgument(valueType, customTypeInfo);
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
        internal void InvokeJsMethod(ExecuteJsMethod executeJsMethod)
        {
            if (!jsMethodsEnable)
                throw new NotEnabledException(
                    $"The JS Method manager is disabled! You need to enable it using {nameof(jsMethodsEnable)}.");
            
            //Get registered method first
            (string? methodName, JsMethodInfo foundMethodInfo) = JsMethods.FirstOrDefault(x => x.Key == executeJsMethod.MethodName);
            if (methodName == null)
                throw new MethodNotFoundException($"Browser tried executing the method '{executeJsMethod.MethodName}', which has not been registered!");

            MethodArgument[]? foundMethodArguments = foundMethodInfo.Arguments;
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
                    MethodArgument matchingArgument = foundMethodArguments![i];

                    if(!(matchingArgument.ValueType == JsValueType.Object && executedArgument.Type == JsValueType.Null)
                       && executedArgument.Type != matchingArgument.ValueType)
                        throw new InvalidArgumentsException($"Invalid argument type! Was excepting '{matchingArgument.ValueType}', but got type of '{executedArgument.Type}'!");

                    object argumentValue = executedArgument.Value;
                    if (matchingArgument.TypeInfo != null && executedArgument.Type == JsValueType.Object)
                    {
                        CustomTypeInfo customTypeInfo = matchingArgument.TypeInfo.Value;
                        
                        JsObjectHolder objectHolder = (JsObjectHolder)executedArgument.Value;
                        if(objectHolder.Keys.Length != customTypeInfo.TypeProperties.Length)
                            throw new InvalidArgumentsException($"Passed in argument object keys count does not match what is excepted!");

                        argumentValue = CreateObjectFromObjectHolder(objectHolder, customTypeInfo);
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
                        customPropertyTypeInfo.TypeInfo!.Value);
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
                
                if (propertyType.IsArray)
                    throw new UnsupportedTypeException("Parameters cannot be an array type!");
                
                KeyValuePair<Type, JsValueType> propertyTypeMatch = typeMatching.FirstOrDefault(x => x.Key == propertyType);
                JsValueType propertyValueType = propertyTypeMatch.Key == null ? JsValueType.Object : propertyTypeMatch.Value;

                //This is an object type, which we need some more info on
                CustomTypeInfo? objectPropertyTypeInfo = null;
                if (propertyValueType == JsValueType.Object)
                    objectPropertyTypeInfo = CreateCustomTypeInfoForType(propertyType);

                propertyTypeInfos[i] =
                    new CustomPropertyTypeInfo(propertyValueType, property.Name, objectPropertyTypeInfo);
            }

            return new CustomTypeInfo(type, propertyTypeInfos);
        }
        
        /// <summary>
        ///     Contains details related to a object's property
        /// </summary>
        internal struct CustomPropertyTypeInfo
        {
            public CustomPropertyTypeInfo(JsValueType valueType, string propertyName, CustomTypeInfo? typeInfo)
            {
                ValueType = valueType;
                PropertyName = propertyName;
                TypeInfo = typeInfo;
            }
            
            /// <summary>
            ///     What <see cref="JsValueType"/> this object is
            /// </summary>
            public JsValueType ValueType { get; set; }
            
            /// <summary>
            ///     The 'Name' of the property
            /// </summary>
            public string PropertyName { get; set; }
            
            /// <summary>
            ///     If this property is a custom object, then <see cref="TypeInfo"/> of this property
            /// </summary>
            public CustomTypeInfo? TypeInfo { get; set; }
        }
        
        /// <summary>
        ///     Contains details to a custom object
        /// </summary>
        internal struct CustomTypeInfo
        {
            public CustomTypeInfo(Type rootType, CustomPropertyTypeInfo[] properties)
            {
                RootType = rootType;
                TypeProperties = properties;
            }
            
            /// <summary>
            ///     The root <see cref="Type"/> of this custom
            /// </summary>
            public Type RootType { get; set; }

            /// <summary>
            ///     All properties on this custom type
            /// </summary>
            public CustomPropertyTypeInfo[] TypeProperties { get; set; }
        }
        
        /// <summary>
        ///     Contains info on a method's argument
        /// </summary>
        internal struct MethodArgument
        {
            public MethodArgument(JsValueType valueType, CustomTypeInfo? typeInfo)
            {
                ValueType = valueType;
                TypeInfo = typeInfo;
            }
            
            /// <summary>
            ///     <see cref="JsValueType"/> this argument is
            /// </summary>
            public JsValueType ValueType { get; set; }
            
            /// <summary>
            ///     If this argument is a custom object, then <see cref="TypeInfo"/> about it
            /// </summary>
            public CustomTypeInfo? TypeInfo { get; set; }
        }
        
        /// <summary>
        ///     Contains info a method that can be invoked by JS
        /// </summary>
        internal struct JsMethodInfo
        {
            /// <summary>
            ///     The <see cref="MethodInfo"/> of the method
            /// </summary>
            public MethodInfo Method { get; set; }
            
            /// <summary>
            ///     Details on the arguments (if any)
            /// </summary>
            public MethodArgument[]? Arguments { get; set; }
            
            /// <summary>
            ///     The object 
            /// </summary>
            public object Target { get; set; }
        }
    }
}