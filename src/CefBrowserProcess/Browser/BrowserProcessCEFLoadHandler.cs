using CefBrowserProcess.Core;
using Xilium.CefGlue;

namespace CefBrowserProcess.Browser
{
    public class BrowserProcessCEFLoadHandler : CefLoadHandler
    {
        private readonly BrowserProcessCEFClient client;

        internal BrowserProcessCEFLoadHandler(BrowserProcessCEFClient client)
        {
            this.client = client;
        }

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        {
            if(frame.IsMain)
                Logger.Debug($"START: {browser?.GetMainFrame().Url}");
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            if(frame.IsMain)
                Logger.Debug($"END: {browser.GetMainFrame().Url}, {httpStatusCode}");
        }

        protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            if(errorCode is CefErrorCode.Aborted 
                or CefErrorCode.BLOCKED_BY_RESPONSE 
                or CefErrorCode.BLOCKED_BY_CLIENT 
                or CefErrorCode.BLOCKED_BY_CSP)
                return;
				
            string html = 
                $@"<style>
@import url('https://fonts.googleapis.com/css2?family=Ubuntu&display=swap');
body {{
font-family: 'Ubuntu', sans-serif;
}}
</style>
<h2>An error occurred while trying to load '{failedUrl}'!</h2>
<p>Error: {errorText}<br>(Code: {(int)errorCode})</p>";
            client.LoadHtml(html);

            Logger.Error($"An error occurred while trying to load '{failedUrl}'! Error: {errorText} (Code: {errorCode})");
        }
    }
}