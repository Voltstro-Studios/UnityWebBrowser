using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    public enum ButtonType
    {
        Back,
        Forward,
        Refresh,
        NavigateUrl
    }
    
    [MessagePackObject]
    public class ButtonEvent : IEventData
    {
        [Key(0)]
        public ButtonType ButtonType { get; set; }

        [Key(1)]
        public string UrlToNavigate { get; set; }
    }
}