namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
    public class PingEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.Ping;
    }
}