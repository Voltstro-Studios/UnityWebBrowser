using System;

namespace UnityWebBrowser.Shared.Events.EngineAction
{
    [Serializable]
    public enum MouseClickType
    {
        Left,
        Middle,
        Right
    }
    
    [Serializable]
    public enum MouseEventType
    {
        Down,
        Up
    }
    
    [Serializable]
    public class MouseClickEvent
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public int MouseClickCount { get; set; }
        public MouseClickType MouseClickType { get; set; }
        public MouseEventType MouseEventType { get; set; }
    }
}