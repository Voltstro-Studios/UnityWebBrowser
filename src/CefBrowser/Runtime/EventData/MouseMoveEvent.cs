using System;

namespace UnityWebBrowser.EventData
{
	[Serializable]
    public class MouseMoveEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseMoveEvent;

	    public int MouseX { get; set; }
	    public int MouseY { get; set; }
    }
}
