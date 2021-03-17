#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
{
	public interface IEventData
    {
	    public EventType EventType { get; set; }
    }
}