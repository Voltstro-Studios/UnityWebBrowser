using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    [MessagePackObject]
    public class LoadHtmlEvent : IEventData
    {
        [Key(0)] public string Html { get; set; }
    }
}