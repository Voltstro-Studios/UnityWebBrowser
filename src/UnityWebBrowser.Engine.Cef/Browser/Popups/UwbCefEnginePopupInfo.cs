using UnityWebBrowser.Engine.Cef.Core;
using UnityWebBrowser.Engine.Shared.Popups;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Popups;
using Xilium.CefGlue;
using Xilium.CefGlue.Platform.Windows;

namespace UnityWebBrowser.Engine.Cef.Browser.Popups;

/// <summary>
///     Cef implementation of <see cref="EnginePopupInfo"/>
/// </summary>
public class UwbCefEnginePopupInfo : EnginePopupInfo
{
    private readonly EnginePopupManager popupManager;
    private readonly UwbCefPopupClient popupClient;
    
    /// <summary>
    ///     Creates a new <see cref="UwbCefEnginePopupInfo"/>
    /// </summary>
    /// <param name="windowInfo"></param>
    /// <param name="settings"></param>
    /// <param name="targetFrameName"></param>
    /// <param name="targetUrl"></param>
    /// <param name="popupFeatures"></param>
    /// <param name="proxySettings"></param>
    /// <param name="popupManager"></param>
    public UwbCefEnginePopupInfo(CefWindowInfo windowInfo, CefBrowserSettings settings, string targetFrameName, 
        string targetUrl, CefPopupFeatures popupFeatures, ProxySettings proxySettings, EnginePopupManager popupManager)
    {
        this.popupManager = popupManager;
        
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
        popupClient = new(proxySettings, DisposeNoClose);
        CefBrowserHost.CreateBrowser(newWindow, popupClient, settings, targetUrl);
    }
    
    public override void ExecuteJs(string js)
    {
        popupClient.ExecuteJs(js);
    }

    public override void Dispose()
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineControlsManager.PostTask(CefThreadId.UI, Dispose);
            return;
        }

        base.Dispose();
        popupClient.Close();
    }

    private void DisposeNoClose()
    {
        popupManager.OnPopupDestroy(this);
    }
}