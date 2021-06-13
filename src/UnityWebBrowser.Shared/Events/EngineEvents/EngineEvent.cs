using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineEvents
{
    [MessagePackObject]
    [Union(0, typeof(OkEvent))]
    [Union(1, typeof(PixelsEvent))]
    public abstract class EngineEvent
    {
    }
}