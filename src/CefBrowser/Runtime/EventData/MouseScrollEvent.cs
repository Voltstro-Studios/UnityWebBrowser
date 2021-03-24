namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
    public class MouseScrollEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseScrollEvent;

	    public int MouseX { get; set; }
	    public int MouseY { get; set; }

	    public int MouseScroll { get; set; }
    }
}
