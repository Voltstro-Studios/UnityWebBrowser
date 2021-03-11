using System;

namespace UnityWebBrowser
{
	[Serializable]
    public class MouseEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.MouseEvent;

	    public int MouseX;
	    public int MouseY;
    }
}
