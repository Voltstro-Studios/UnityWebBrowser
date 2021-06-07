using MessagePack;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    [MessagePackObject]
    public class LoadHtmlEvent : IEventData
    {
        [Key(0)] public string Html { get; set; }
    }
}