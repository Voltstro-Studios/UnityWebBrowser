#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
{
    public class PingEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.Ping;
    }
}