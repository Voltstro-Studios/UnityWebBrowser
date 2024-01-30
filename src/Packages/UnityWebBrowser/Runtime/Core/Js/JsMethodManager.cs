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
        
        private readonly Dictionary<string, JsMethodInfo> jsMethod = new();
        
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
            
            if (jsMethod.ContainsKey(name))
                throw new ArgumentException($"A method of the name {name} is already registered!", nameof(name));
            
            ParameterInfo[] methodParameters = methodInfo.GetParameters();

            JsValue[]? argumentTypes = null;
            if (methodParameters.Length > 0)
            {
                argumentTypes = new JsValue[methodParameters.Length];
                for (int i = 0; i < methodParameters.Length; i++)
                {
                    Type type = methodParameters[i].ParameterType;
                    KeyValuePair<Type, JsValueType> typeMatch = typeMatching.FirstOrDefault(x => x.Key == type);

                    JsValueType valueType = typeMatch.Key == null ? JsValueType.Object : typeMatch.Value;
                    argumentTypes[i] = new JsValue
                    {
                        Type = valueType
                    };
                }
            }
            
            //method.Method
            jsMethod.Add(name, new JsMethodInfo
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
            (string? methodName, JsMethodInfo foundMethodInfo) = jsMethod.FirstOrDefault(x => x.Key == executeJsMethod.MethodName);
            if (methodName == null)
                throw new MethodNotFoundException($"Browser tried executing the method '{methodName}', which has not been registered!");

            JsValue[]? foundMethodArguments = foundMethodInfo.Arguments;
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
                    JsValue matchingArgument = foundMethodArguments![i];

                    if (executedArgument.Type != matchingArgument.Type)
                        throw new InvalidArgumentsException($"Invalid argument type! Was excepting '{matchingArgument.Type}', but got type of '{executedArgument.Type}'!");

                    arguments[i] = executedArgument.Value;
                }
            }

            //Invoke method
            foundMethodInfo.Method.Invoke(foundMethodInfo.Target, arguments);
        }
        
        private struct JsMethodInfo
        {
            public MethodInfo Method { get; set; }
            public JsValue[]? Arguments { get; set; }
            public object Target { get; set; }
        }
    }
}