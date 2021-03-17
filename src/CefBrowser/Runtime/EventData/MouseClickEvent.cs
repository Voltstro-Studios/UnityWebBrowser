#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
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

	    public int MouseX;
	    public int MouseY;

	    public int MouseClickCount;

	    public MouseClickType MouseClickType;
	    public MouseEventType MouseEventType;
    }
}