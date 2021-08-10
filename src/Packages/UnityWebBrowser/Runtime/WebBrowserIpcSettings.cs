using System;
using UnityEngine;

namespace UnityWebBrowser
{
    [Serializable]
    public class WebBrowserIpcSettings
    {
        public bool preferPipes = true;
        
        public string outPipeName = "UnityWebBrowserOut";
        public string inPipeName = "UnityWebBrowserIn";

        public int outPort = 5555;
        public int inPort = 5555;
        
        /// <summary>
        ///     Timeout time for connection (in milliseconds)
        /// </summary>
        [Tooltip("Timeout time for connection (in milliseconds)")]
        public int connectionTimeout = 100000;
    }
}