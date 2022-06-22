using System;
using VoltRpc.Proxy;

namespace UnityWebBrowser.Shared.Popups;

/// <summary>
///     Interface for controls of popup from the engine's POV
/// </summary>
[GenerateProxy(GeneratedName = "PopupEngineControls", GeneratedNamespace = "UnityWebBrowser.Shared.Popups")]
public interface IPopupEngineControls
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