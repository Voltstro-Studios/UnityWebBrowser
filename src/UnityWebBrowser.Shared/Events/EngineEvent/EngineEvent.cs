using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineEvent
{
    [MessagePackObject]
    [Union(0, typeof(OnUrlChangeEvent))]
    public abstract class EngineEvent
    {
    }
}