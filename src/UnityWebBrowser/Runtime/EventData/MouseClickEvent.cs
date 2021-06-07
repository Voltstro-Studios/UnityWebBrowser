using MessagePack;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    public enum MouseClickType
    {
        Left,
        Middle,
        Right
    }

#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    public enum MouseEventType
    {
        Down,
        Up
    }

#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    [MessagePackObject]
    public class MouseClickEvent : IEventData
    {
        [Key(0)]
        public int MouseX { get; set; }

        [Key(1)]
        public int MouseY { get; set; }

        [Key(2)]
        public int MouseClickCount { get; set; }

        [Key(3)]
        public MouseClickType MouseClickType { get; set; }

        [Key(4)]
        public MouseEventType MouseEventType { get; set; }

        [Key(5)]
        public EventType EventType { get; set; } = EventType.MouseClickEvent;
    }
}