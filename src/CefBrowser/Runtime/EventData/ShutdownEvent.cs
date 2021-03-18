using System;

namespace UnityWebBrowser.EventData
{
    [Serializable]
    public class ShutdownEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.Shutdown;
    }
}