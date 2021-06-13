using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineActions
{
    [MessagePackObject]
    public class NavigateUrlEvent : EngineActionEvent
    {
        [Key(0)] public string Url { get; set; }
    }
}