using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoltstroStudios.UnityWebBrowser.Shared.Popups;
using VoltRpc.Communication;

#nullable enable
namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Popups;

/// <summary>
///     Manager for popups on the engine side
/// </summary>
public class EnginePopupManager : IPopupClientControls
{
    private Client? client;
    private IPopupEngineControls? engineControls;

    private readonly List<EnginePopupInfo> popups;

    /// <summary>
    ///     Creates a new <see cref="EnginePopupManager"/> instance
    /// </summary>
    public EnginePopupManager()
    {
        popups = new List<EnginePopupInfo>();
    }

    /// <summary>
    ///     Call when you have a popup
    /// </summary>
    /// <param name="enginePopupInfo"></param>
    public void OnPopup(EnginePopupInfo enginePopupInfo)
    {
        popups.Add(enginePopupInfo);
        if (client is { IsConnected: true })
            engineControls?.OnPopup(enginePopupInfo.PopupGuid);
    }

    /// <summary>
    ///     Call when you want to destroy a popup
    /// </summary>
    /// <param name="enginePopupInfo"></param>
    public void OnPopupDestroy(EnginePopupInfo enginePopupInfo)
    {
        popups.Remove(enginePopupInfo);
        if (client is { IsConnected: true })
            engineControls?.OnPopupDestroy(enginePopupInfo.PopupGuid);
    }
    
    /// <inheritdoc />
    public void PopupClose(Guid guid)
    {
        EnginePopupInfo popupInfo = popups.First(x => x.PopupGuid == guid);
        popups.Remove(popupInfo);
        _ = Task.Run(popupInfo.Dispose);
    }

    /// <inheritdoc />
    public void PopupExecuteJs(Guid guid, string js)
    {
        EnginePopupInfo popupInfo = popups.First(x => x.PopupGuid == guid);
        popupInfo.ExecuteJs(js);
    }
    
    /// <summary>
    ///     Setup everything for IPC
    /// </summary>
    /// <param name="ipcClient"></param>
    internal void SetIpcClient(Client ipcClient)
    {
        client = ipcClient;
        engineControls = new PopupEngineControls(client);
    }
}