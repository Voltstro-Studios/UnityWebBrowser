using System;
using UnityWebBrowser.Engine.Cef.Core;
using UnityWebBrowser.Shared;
using Xilium.CefGlue;
using Xilium.CefGlue.Platform.Windows;

namespace UnityWebBrowser.Engine.Cef.Browser;

/// <summary>
///     <see cref="CefLifeSpanHandler" /> implementation
/// </summary>
public class UwbCefLifespanHandler : CefLifeSpanHandler
{
    public event Action<CefBrowser> AfterCreated;
    public event Action<string> OnPopup;

    private readonly ProxySettings proxySettings;
    
    private readonly PopupAction popupAction;

    public UwbCefLifespanHandler(PopupAction popupAction, ProxySettings proxySettings)
    {
        this.proxySettings = proxySettings;
        this.popupAction = popupAction;
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
        OnPopup?.Invoke(targetUrl);

        switch (popupAction)
        {
            case PopupAction.Ignore:
                break;
            case PopupAction.OpenExternalWindow:
                //Create new window info
                CefWindowInfo newWindow = CefWindowInfo.Create();
                
                //Set the window as a popup and parent it to the existing 'window'.
                newWindow.SetAsPopup(windowInfo.Handle, targetFrameName);
                
                //Set sizes and positions
                if (popupFeatures.X.HasValue)
                    newWindow.X = popupFeatures.X.Value;
                if (popupFeatures.Y.HasValue)
                    newWindow.Y = popupFeatures.Y.Value;
                if (popupFeatures.Width.HasValue)
                    newWindow.Width = popupFeatures.Width.Value;
                if (popupFeatures.Height.HasValue)
                    newWindow.Height = popupFeatures.Height.Value;

                //Scrollbars
                if (popupFeatures.ScrollbarsVisible)
                    newWindow.Style |= WindowStyle.WS_HSCROLL | WindowStyle.WS_VSCROLL;

                //Create a new client for it, and properly create the window
                UwbCefClientPopup clientPopup = new UwbCefClientPopup(proxySettings);
                CefBrowserHost.CreateBrowser(newWindow, clientPopup, settings, targetUrl);
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