namespace UnityWebBrowser.EventData
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

	    public ButtonType ButtonType { get; set; }

	    public string UrlToNavigate { get; set; }
    }
}
