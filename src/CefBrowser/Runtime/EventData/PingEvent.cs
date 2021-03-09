namespace UnityWebBrowser
{
    public class PingEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.Ping;
    }
}