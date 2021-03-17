#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
{
    public class MouseScrollEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseScrollEvent;

	    public int MouseX;
	    public int MouseY;

	    public int MouseScroll;
    }
}
