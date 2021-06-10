using UnityWebBrowser.Engine.Cef.Models;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser
{
    /// <summary>
    ///     <see cref="CefRenderHandler"/> implementation
    /// </summary>
    public class BrowserProcessCEFRequestHandler : CefRequestHandler
    {
        private readonly ProxySettings proxySettings;
        
        public BrowserProcessCEFRequestHandler(ProxySettings proxySettings)
        {
            this.proxySettings = proxySettings;
        }

        protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request,
            bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return null;
        }

        protected override bool GetAuthCredentials(CefBrowser browser, string originUrl, bool isProxy, string host, int port, string realm,
            string scheme, CefAuthCallback callback)
        {
            if (isProxy)
            {
                callback.Continue(proxySettings.Username, proxySettings.Password);
            }
				
            return base.GetAuthCredentials(browser, originUrl, isProxy, host, port, realm, scheme, callback);
        }
    }
}