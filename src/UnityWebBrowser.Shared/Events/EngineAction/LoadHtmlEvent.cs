using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineAction
{
    [MessagePackObject]
    public class LoadHtmlEvent : EngineActionEvent
    {
        [Key(0)] public string Html { get; set; }
    }
}