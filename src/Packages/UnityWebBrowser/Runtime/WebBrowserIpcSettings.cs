using System;
using UnityEngine;

namespace UnityWebBrowser
{
    /// <summary>
    ///     Settings related to IPC
    /// </summary>
    [Serializable]
    public class WebBrowserIpcSettings
    {
        /// <summary>
        ///     Use Pipes or TCP. Some Unity platforms may have issues with pipes.
        /// </summary>
        [Tooltip("Use Pipes or TCP. Some Unity platforms may have issues with pipes.")]
        public bool preferPipes = true;
        
        /// <summary>
        ///     Whats the name of the pipe for outwards communication
        /// </summary>
        [Tooltip(" Whats the name of the pipe for outwards communication")]
        public string outPipeName = "UnityWebBrowserOut";
        
        /// <summary>
        ///      Whats the name of the pipe for inwards communication
        /// </summary>
        [Tooltip("Whats the name of the pipe for inwards communication")]
        public string inPipeName = "UnityWebBrowserIn";

        /// <summary>
        ///     Whats the port for outwards communication
        /// </summary>
        [Tooltip("Whats the port for outwards communication")]
        public int outPort = 5555;
        
        /// <summary>
        ///     Whats the port for inwards communication
        /// </summary>
        [Tooltip("Whats the port for inwards communication")]
        public int inPort = 5555;
        
        /// <summary>
        ///     Timeout time for connection (in milliseconds)
        /// </summary>
        [Tooltip("Timeout time for connection (in milliseconds)")]
        public int connectionTimeout = 100000;
    }
}