using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
    public enum ButtonType
    {
        Back,
        Forward,
        Refresh,
        NavigateUrl
    }

#if !BROWSER_PROCESS
    [Preserve]
#endif
    public class ButtonEvent : IEventData
    {
        public ButtonType ButtonType { get; set; }

        public string UrlToNavigate { get; set; }
        public EventType EventType { get; set; } = EventType.ButtonEvent;
    }
}