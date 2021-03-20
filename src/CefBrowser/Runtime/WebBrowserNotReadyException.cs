using System;

namespace UnityWebBrowser
{
	/// <summary>
	///		The web browser is not ready exception
	/// </summary>
    public class WebBrowserNotReadyException : Exception
    {
	    public WebBrowserNotReadyException()
	    {
	    }

	    public WebBrowserNotReadyException(string message) : base(message)
	    {
	    }
    }
}