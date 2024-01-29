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
            uiBasic.browserClient.RegisterJsMethod("Test", TestMethod);
        }

        private void TestMethod()
        {
            Debug.Log("Hello from test method!");
        }
    }
}