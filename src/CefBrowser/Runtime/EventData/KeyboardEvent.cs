using System;

#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
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