using MessagePack;

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
    [UnityEngine.Scripting.Preserve]
#endif
    [MessagePackObject]
    public class ButtonEvent : IEventData
    {
        [Key(0)]
        public ButtonType ButtonType { get; set; }

        [Key(1)]
        public string UrlToNavigate { get; set; }
    }
}