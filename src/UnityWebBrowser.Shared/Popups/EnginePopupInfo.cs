using System;

namespace UnityWebBrowser.Shared.Popups;

public abstract class EnginePopupInfo : IDisposable
{
    public EnginePopupInfo()
    {
        PopupGuid = Guid.NewGuid();
    }

    public EnginePopupInfo(Guid guid)
    {
        PopupGuid = guid;
    }

    internal readonly Guid PopupGuid;
    
    public virtual void Dispose()
    {
    }
}