namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
    public class ShutdownEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.Shutdown;
    }
}