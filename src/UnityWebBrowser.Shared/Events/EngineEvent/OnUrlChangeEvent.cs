using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineEvent
{
    [MessagePackObject]
    public class OnUrlChangeEvent : EngineEvent
    {
        [Key(0)]
        public string NewUrl { get; set; }
    }
}