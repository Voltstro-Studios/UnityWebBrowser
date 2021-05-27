using System;
using Xilium.CefGlue;

namespace CefBrowserProcess.Browser
{
    /// <summary>
    ///     <see cref="CefLifeSpanHandler"/> implementation
    /// </summary>
    public class BrowserProcessCEFLifespanHandler : CefLifeSpanHandler
    {
        public event Action<CefBrowser> AfterCreated; 

        protected override void OnAfterCreated(CefBrowser browser)
        {
            AfterCreated?.Invoke(browser);
        }

        protected override bool DoClose(CefBrowser browser)
        {
            return false;
        }
    }
}