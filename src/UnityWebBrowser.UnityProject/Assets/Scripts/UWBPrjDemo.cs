// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using UnityEngine;

namespace VoltstroStudios.UnityWebBrowser.Prj
{
    public class UWBPrjDemo : MonoBehaviour
    {
        [SerializeField]
        private WebBrowserUIBasic uiBasic;
        
        public void Awake()
        {
            uiBasic.browserClient.OnClientInitialized += BrowserClientOnOnClientInitialized;
        }

        private void BrowserClientOnOnClientInitialized()
        {
            uiBasic.browserClient.RegisterJsMethod("Test", TestMethod);
            uiBasic.browserClient.RegisterJsMethod<int>("TestInt", TestMethodInt);
            uiBasic.browserClient.RegisterJsMethod<int, string>("TestIntString", TestMethodIntString);
            uiBasic.browserClient.RegisterJsMethod<string>("TestString", TestMethodString);
            uiBasic.browserClient.RegisterJsMethod<DateTime>("TestDate", TestMethodDate);
            uiBasic.browserClient.RegisterJsMethod<TestClass>("TestObject", TestMethodObject);
            uiBasic.browserClient.RegisterJsMethod<TestClassChild>("TestObjectChild", TestMethodObjectChild);
        }

        private void TestMethod()
        {
            Debug.Log("Hello from test method!");
        }

        private void TestMethodInt(int value)
        {
            Debug.Log($"Hello from test method! Value was {value}.");
        }
        
        private void TestMethodString(string value)
        {
            Debug.Log($"Hello from test method! Value was {value}.");
        }
        
        private void TestMethodIntString(int intValue, string stringValue)
        {
            Debug.Log($"Hello from test method! Values was {intValue} and {stringValue}.");
        }
        
        private void TestMethodDate(DateTime value)
        {
            DateTime localTime = value.ToLocalTime();
            Debug.Log($"Hello from test method! Value in UTC time was {value:yyyy-MM-dd HH:mm:ss zzzz}. Value in local time was {localTime:yyyy-MM-dd HH:mm:ss zzzz}.");
        }

        private void TestMethodObject(TestClass test)
        { 
            Debug.Log($"Hello from test method! Value on TestClass was {test.Test}.");   
        }

        private void TestMethodObjectChild(TestClassChild test)
        {
            Debug.Log($"Hello from test method! Value on TestClassChild was {test.What}, TestClass was {test.Test.Test}.");
        }
    }

    public class TestClass
    {
        public string Test { get; set; }
    }

    public class TestClassChild
    {
        public string What { get; set; }
        
        public TestClass Test { get; set; }
    }
}