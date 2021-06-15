using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineActions
{
    [MessagePackObject]
    public class LoadUrlEvent : EngineActionEvent
    {
        [Key(0)] public string Url { get; set; }
    }
}