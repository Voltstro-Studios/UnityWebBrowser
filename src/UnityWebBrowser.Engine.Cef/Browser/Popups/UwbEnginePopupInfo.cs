using UnityWebBrowser.Engine.Cef.Core;
using UnityWebBrowser.Engine.Shared.Popups;
using UnityWebBrowser.Shared;
using UnityWebBrowser.Shared.Popups;
using Xilium.CefGlue;
using Xilium.CefGlue.Platform.Windows;

namespace UnityWebBrowser.Engine.Cef.Browser.Popups;

public class UwbEnginePopupInfo : EnginePopupInfo
{
    private readonly EnginePopupManager popupManager;
    private readonly UwbCefClientPopup client;
    
    public UwbEnginePopupInfo(CefWindowInfo windowInfo, CefBrowserSettings settings, string targetFrameName, 
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
        client = new(proxySettings, DisposeNoClose);
        CefBrowserHost.CreateBrowser(newWindow, client, settings, targetUrl);
    }

    public override void ExecuteJs(string js)
    {
        client.ExecuteJs(js);
    }

    public override void Dispose()
    {
        if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        {
            CefEngineManager.PostTask(CefThreadId.UI, Dispose);
            return;
        }

        base.Dispose();
        client.Close();
    }

    public void DisposeNoClose()
    {
        popupManager.OnPopupDestroy(this);
    }

    //TODO: Shutdown, ExecuteJs, etc
}