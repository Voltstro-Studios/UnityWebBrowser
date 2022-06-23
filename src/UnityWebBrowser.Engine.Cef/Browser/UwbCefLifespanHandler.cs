using System;
using UnityWebBrowser.Engine.Cef.Browser.Popups;
using UnityWebBrowser.Engine.Cef.Core;
using UnityWebBrowser.Engine.Shared.Core.Logging;
using UnityWebBrowser.Engine.Shared.Popups;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Popups;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefLifeSpanHandler" /> implementation
/// </summary>
public class UwbCefLifespanHandler : CefLifeSpanHandler
{
    public event Action<CefBrowser> AfterCreated;

    private readonly EnginePopupManager popupManager;
    private readonly ProxySettings proxySettings;
    
    private readonly PopupAction popupAction;

    public UwbCefLifespanHandler(PopupAction popupAction, EnginePopupManager enginePopupManager, ProxySettings proxySettings)
    {
        this.proxySettings = proxySettings;
        this.popupAction = popupAction;
        popupManager = enginePopupManager;
    }

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
        CefLoggerWrapper.Debug($"Popup: {targetFrameName}({targetUrl})");

        switch (popupAction)
        {
            case PopupAction.Ignore:
                break;
            case PopupAction.OpenExternalWindow:
                try
                {
                    popupManager.OnPopup(new UwbCefEnginePopupInfo(windowInfo, settings, targetFrameName, targetUrl,
                        popupFeatures, proxySettings, popupManager));
                }
                catch (Exception ex)
                {
                    Logger.Error($"Fucked! {ex.Message} {ex.StackTrace}");
                }
                break;
            case PopupAction.Redirect:
                frame.LoadUrl(targetUrl);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return true;
    }

    protected override bool DoClose(CefBrowser browser)
    {
        return false;
    }
}