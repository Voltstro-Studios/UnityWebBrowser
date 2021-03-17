#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
{
	public enum ButtonType
	{
		Back,
		Forward,
		Refresh,
		NavigateUrl
	}

    public class ButtonEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.ButtonEvent;

	    public ButtonType ButtonType;

	    public string UrlToNavigate;
    }
}
