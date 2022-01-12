using System;

namespace UnityWebBrowser
{
    /// <summary>
    ///     An <see cref="Exception"/> related to when you are trying to do something when the engine is not running
    /// </summary>
    public sealed class WebBrowserIsNotConnectedException : Exception
    {
        internal WebBrowserIsNotConnectedException(string message) : base(message)
        {
        }
    }
}