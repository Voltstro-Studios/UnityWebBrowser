// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltRpc.Proxy;

namespace VoltstroStudios.UnityWebBrowser.Shared.Popups;

/// <summary>
///     Interface for controls of popup from the engine's POV
/// </summary>
[GenerateProxy(GeneratedName = "PopupEngineControls", GeneratedNamespace = "VoltstroStudios.UnityWebBrowser.Shared.Popups")]
internal interface IPopupEngineControls
{
    /// <summary>
    ///     Called when a popup is created
    /// </summary>
    public void OnPopup(Guid guid);

    /// <summary>
    ///     Called when a popup is destroyed
    /// </summary>
    /// <param name="guid"></param>
    public void OnPopupDestroy(Guid guid);
}