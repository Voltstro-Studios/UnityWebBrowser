using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [Preserve]
#endif
    public enum MouseClickType
    {
        Left,
        Middle,
        Right
    }

#if !BROWSER_PROCESS
    [Preserve]
#endif
    public enum MouseEventType
    {
        Down,
        Up
    }

#if !BROWSER_PROCESS
    [Preserve]
#endif
    public class MouseClickEvent : IEventData
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }

        public int MouseClickCount { get; set; }

        public MouseClickType MouseClickType { get; set; }
        public MouseEventType MouseEventType { get; set; }
        public EventType EventType { get; set; } = EventType.MouseClickEvent;
    }
}