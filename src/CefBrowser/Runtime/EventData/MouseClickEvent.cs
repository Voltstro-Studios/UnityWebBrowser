namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
	public enum MouseClickType
	{
		Left,
		Middle,
		Right
	}

#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
	public enum MouseEventType
	{
		Down,
		Up
	}

#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
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