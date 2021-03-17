using System;

#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
{
	[Serializable]
    public class MouseMoveEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseMoveEvent;

	    public int MouseX;
	    public int MouseY;
    }
}
