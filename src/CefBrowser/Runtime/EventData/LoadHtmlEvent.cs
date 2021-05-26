namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    public class LoadHtmlEvent : IEventData
    {
        public string Html { get; set; }
        public EventType EventType { get; set; } = EventType.LoadHtmlEvent;
    }
}