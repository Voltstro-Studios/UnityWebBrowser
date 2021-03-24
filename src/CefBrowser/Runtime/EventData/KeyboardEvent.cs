using System;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
    public class KeyboardEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.KeyboardEvent;

	    public int[] KeysUp { get; set; }
	    public int[] KeysDown { get; set; }

	    public string Chars { get; set; }
    }
}