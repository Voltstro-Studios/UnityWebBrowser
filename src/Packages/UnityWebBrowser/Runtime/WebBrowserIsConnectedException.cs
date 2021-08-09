using System;

namespace UnityWebBrowser
{
    public class WebBrowserIsConnectedException : Exception
    {
        public WebBrowserIsConnectedException(string message)
            : base(message)
        {
        }
    }
}