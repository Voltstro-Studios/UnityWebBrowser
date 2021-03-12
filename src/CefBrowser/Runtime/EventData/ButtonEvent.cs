namespace UnityWebBrowser
{
	public enum ButtonType
	{
		Back,
		Forward,
		NavigateUrl
	}

    public class ButtonEvent : IEventData
    {
	    public EventType EventType { get; set; } = EventType.ButtonEvent;

	    public ButtonType ButtonType;

	    public string UrlToNavigate;
    }
}
