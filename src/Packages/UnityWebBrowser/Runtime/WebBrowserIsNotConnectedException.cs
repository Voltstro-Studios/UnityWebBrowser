using System;

namespace UnityWebBrowser
{
	/// <summary>
	///     The web browser is not ready exception
	/// </summary>
	public class WebBrowserIsNotConnectedException : Exception
    {
        public WebBrowserIsNotConnectedException()
        {
        }

        public WebBrowserIsNotConnectedException(string message) : base(message)
        {
        }
    }
}