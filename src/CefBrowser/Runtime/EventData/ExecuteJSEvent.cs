namespace UnityWebBrowser.EventData
{
    public class ExecuteJsEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.ExecuteJsEvent;

	    public string Js;
    }
}