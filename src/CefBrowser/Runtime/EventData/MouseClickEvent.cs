namespace UnityWebBrowser.EventData
{
	public enum MouseClickType
	{
		Left,
		Middle,
		Right
	}

	public enum MouseEventType
	{
		Down,
		Up
	}

    public class MouseClickEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseClickEvent;

	    public int MouseX { get; set; }
	    public int MouseY { get; set; }

	    public int MouseClickCount { get; set; }

	    public MouseClickType MouseClickType { get; set; }
	    public MouseEventType MouseEventType { get; set; }
    }
}