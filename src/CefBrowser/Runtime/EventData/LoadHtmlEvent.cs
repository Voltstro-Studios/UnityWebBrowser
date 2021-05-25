using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [Preserve]
#endif
    public class LoadHtmlEvent : IEventData
    {
        public string Html { get; set; }
        public EventType EventType { get; set; } = EventType.LoadHtmlEvent;
    }
}