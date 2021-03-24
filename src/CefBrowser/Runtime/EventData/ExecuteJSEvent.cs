namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
    public class ExecuteJsEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.ExecuteJsEvent;

	    public string Js { get; set; }
    }
}