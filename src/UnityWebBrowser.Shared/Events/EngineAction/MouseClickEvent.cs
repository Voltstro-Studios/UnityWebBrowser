using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineAction
{
    public enum MouseClickType
    {
        Left,
        Middle,
        Right
    }
    
    public enum MouseEventType
    {
        Down,
        Up
    }
    
    [MessagePackObject]
    public class MouseClickEvent : EngineActionEvent
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
    }
}