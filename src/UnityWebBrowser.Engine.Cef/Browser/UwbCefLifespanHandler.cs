using System;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefLifeSpanHandler" /> implementation
/// </summary>
public class UwbCefLifespanHandler : CefLifeSpanHandler
{
    public event Action<CefBrowser> AfterCreated;

    protected override void OnAfterCreated(CefBrowser browser)
    {
        AfterCreated?.Invoke(browser);
    }

    protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl,
        string targetFrameName,
        CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures,
        CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings,
        ref CefDictionaryValue extraInfo,
        ref bool noJavascriptAccess)
    {
        frame.LoadUrl(targetUrl);
        return true;
    }

    protected override bool DoClose(CefBrowser browser)
    {
        return false;
    }
}