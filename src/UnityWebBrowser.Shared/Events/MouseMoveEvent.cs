using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    [MessagePackObject]
    public class MouseMoveEvent : IEventData
    {
        [Key(0)]
        public int MouseX { get; set; }

        [Key(1)]
        public int MouseY { get; set; }
    }
}