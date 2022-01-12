using System;

namespace UnityWebBrowser
{
    /// <summary>
    ///     An <see cref="Exception"/> related to when you are trying to change something that cannot be changed when the engine is already running
    /// </summary>
    public sealed class WebBrowserIsConnectedException : Exception
    {
        internal WebBrowserIsConnectedException(string message)
            : base(message)
        {
        }
    }
}