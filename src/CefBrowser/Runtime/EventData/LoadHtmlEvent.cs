using MessagePack;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    [MessagePackObject]
    public class LoadHtmlEvent : IEventData
    {
        public string Html { get; set; }
    }
}