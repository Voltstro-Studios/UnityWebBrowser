using System;

namespace UnityWebBrowser.EventData
{
    [Serializable]
    public class KeyboardEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.KeyboardEvent;

	    public int[] KeysUp { get; set; }
	    public int[] KeysDown { get; set; }

	    public string Chars { get; set; }
    }
}