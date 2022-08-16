// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using UnityWebBrowser.Engine.Cef.Core;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Popups;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;
using Xilium.CefGlue;

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
    /// <param name="proxySettings"></param>
    /// <param name="popupManager"></param>
    public UwbCefEnginePopupInfo(EnginePopupManager popupManager, ProxySettings proxySettings, ref CefClient client)
    {
        this.popupManager = popupManager;

        //Create a new client for it, and properly create the window
        popupClient = new UwbCefPopupClient(proxySettings, DisposeNoClose);
        client = popupClient;
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