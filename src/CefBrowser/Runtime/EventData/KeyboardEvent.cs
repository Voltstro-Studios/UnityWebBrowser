using System;

namespace UnityWebBrowser.EventData
{
    [Serializable]
    public class KeyboardEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.KeyboardEvent;

	    public int[] KeysUp;
	    public int[] KeysDown;

	    public string Chars;
    }
}