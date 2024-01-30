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
        
        public void Start()
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
            Debug.Log($"Hello from test method! Value was {value}.");
        }
    }
}