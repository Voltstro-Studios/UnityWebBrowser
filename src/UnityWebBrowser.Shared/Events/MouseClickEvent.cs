namespace UnityWebBrowser.Shared.Events
{
    public enum MouseClickType : byte
    {
        Left,
        Middle,
        Right
    }

    public enum MouseEventType : byte
    {
        Down,
        Up
    }

    public struct MouseClickEvent
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public int MouseClickCount { get; set; }
        public MouseClickType MouseClickType { get; set; }
        public MouseEventType MouseEventType { get; set; }
    }
}