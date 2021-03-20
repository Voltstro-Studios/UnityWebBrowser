namespace UnityWebBrowser.EventData
{
    public class LoadHtmlEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.LoadHtmlEvent;

	    public string Html;
    }
}