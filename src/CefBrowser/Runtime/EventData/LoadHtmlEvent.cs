namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
    public class LoadHtmlEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.LoadHtmlEvent;

	    public string Html { get; set; }
    }
}