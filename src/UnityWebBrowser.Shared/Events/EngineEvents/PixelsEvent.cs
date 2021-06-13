using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineEvents
{
    [MessagePackObject]
    public class PixelsEvent : EngineEvent
    {
        [Key(0)] public byte[] Pixels { get; set; }
    }
}