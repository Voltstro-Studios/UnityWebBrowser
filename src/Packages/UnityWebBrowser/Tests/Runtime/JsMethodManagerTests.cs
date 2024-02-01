// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using VoltstroStudios.UnityWebBrowser.Core.Js;
using VoltstroStudios.UnityWebBrowser.Shared.Js;

namespace VoltstroStudios.UnityWebBrowser.Tests
{
    public class JsMethodManagerTests
    {
        [Test]
        public void MethodNoArgumentsTest()
        {
            JsMethodManager methodManager = new();
            Action action = () => {};
            
            methodManager.RegisterJsMethod("Test", action.Method, this);
            Assert.That(methodManager.JsMethods.Count, Is.Not.Zero);

            (string methodKey, JsMethodManager.JsMethodInfo jsMethodInfo) = methodManager.JsMethods.FirstOrDefault(x => x.Key == "Test");
            Assert.That(methodKey, Is.Not.Null);

            Assert.That(jsMethodInfo.Target, Is.EqualTo(this));
            Assert.That(jsMethodInfo.Method, Is.EqualTo(action.Method));
            Assert.That(jsMethodInfo.Arguments, Is.Null);
        }

        [Test]
        public void MethodObjectArgumentsTest()
        {
            JsMethodManager methodManager = new();
            
            Action<TestObject> action = test => {};
            
            methodManager.RegisterJsMethod("Test", action.Method, this);
            Assert.That(methodManager.JsMethods.Count, Is.Not.Zero);

            (string methodKey, JsMethodManager.JsMethodInfo jsMethodInfo) = methodManager.JsMethods.FirstOrDefault(x => x.Key == "Test");
            
            //Ensure values are correct
            Assert.That(methodKey, Is.Not.Null);

            Assert.That(jsMethodInfo.Target, Is.EqualTo(this));
            Assert.That(jsMethodInfo.Method, Is.EqualTo(action.Method));
            Assert.That(jsMethodInfo.Arguments, Is.Not.Null);
            
            JsMethodManager.CustomPropertyTypeInfo argument = jsMethodInfo.Arguments[0];
            Assert.That(argument.ValueType, Is.EqualTo(JsValueType.Object));
            Assert.That(argument.CustomTypeInfo, Is.Not.Null);
            Assert.That(argument.CustomTypeInfo.RootType, Is.EqualTo(typeof(TestObject)));
            Assert.That(argument.CustomTypeInfo.TypeProperties.Length, Is.EqualTo(1));

            JsMethodManager.CustomPropertyTypeInfo typeProperties = argument.CustomTypeInfo.TypeProperties[0];
            Assert.That(typeProperties.ValueType, Is.EqualTo(JsValueType.String));
            Assert.That(typeProperties.CustomTypeInfo, Is.Null);
            Assert.That(typeProperties.PropertyName, Is.EqualTo("Test"));
        }
        

        public class TestObject
        {
            public string Test { get; set; }
        }
    }
}