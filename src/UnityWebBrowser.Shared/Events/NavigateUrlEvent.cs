using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    [MessagePackObject]
    public class NavigateUrlEvent : EventData
    {
        [Key(0)] public string Url { get; set; }
    }
}