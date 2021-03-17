using System;

#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
{
    [Serializable]
    public class ShutdownEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.Shutdown;
    }
}