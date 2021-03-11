namespace UnityWebBrowser
{
    public class MouseScrollEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseScrollEvent;

	    public int MouseX;
	    public int MouseY;

	    public int MouseScroll;
    }
}
