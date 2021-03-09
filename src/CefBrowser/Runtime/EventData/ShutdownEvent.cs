using System;

namespace UnityWebBrowser
{
    [Serializable]
    public class ShutdownEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.Shutdown;
    }
}