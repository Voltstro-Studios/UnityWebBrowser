// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using UnityWebBrowser.Engine.Cef.Core;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Popups;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser.Popups;

/// <summary>
///     Cef implementation of <see cref="EnginePopupInfo" />
/// </summary>
public class UwbCefEnginePopupInfo : EnginePopupInfo
{
    private readonly UwbCefPopupClient popupClient;
    private readonly EnginePopupManager popupManager;

    /// <summary>
    ///     Creates a new <see cref="UwbCefEnginePopupInfo" />
    /// </summary>
    /// <param name="proxySettings"></param>
    /// <param name="popupManager"></param>
    /// <param name="client"></param>
    /// <param name="ignoreSslErrors"></param>
    /// <param name="ignoreSslErrorsDomains"></param>
    public UwbCefEnginePopupInfo(
        EnginePopupManager popupManager,
        ProxySettings proxySettings,
        ref CefClient client,
        bool ignoreSslErrors,
        string[] ignoreSslErrorsDomains)
    {
        this.popupManager = popupManager;

        //Create a new client for it, and properly create the window
        popupClient = new UwbCefPopupClient(proxySettings, DisposeNoClose, ignoreSslErrors, ignoreSslErrorsDomains);
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
            CefActionTask.PostTask(CefThreadId.UI, Dispose);
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