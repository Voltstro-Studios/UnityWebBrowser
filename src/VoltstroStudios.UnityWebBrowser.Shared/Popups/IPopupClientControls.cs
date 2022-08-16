// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltRpc.Proxy;

namespace VoltstroStudios.UnityWebBrowser.Shared.Popups;

/// <summary>
///     Interface for popup controls from a client
/// </summary>
[GenerateProxy(GeneratedName = "PopupClientControls", GeneratedNamespace = "VoltstroStudios.UnityWebBrowser.Shared.Popups")]
public interface IPopupClientControls
{
    /// <summary>
    ///     Closes a popup
    /// </summary>
    /// <param name="guid"></param>
    public void PopupClose(Guid guid);

    /// <summary>
    ///     Executes js
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="js"></param>
    public void PopupExecuteJs(Guid guid, string js);
}