using System;

namespace UnityWebBrowser
{
	[Serializable]
    public class MouseMoveEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseMoveEvent;

	    public int MouseX;
	    public int MouseY;
    }
}
