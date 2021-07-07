using System;

namespace UnityWebBrowser.Shared.Events.EngineAction
{
    [Serializable]
    public class MouseScrollEvent
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public int MouseScroll { get; set; }
    }
}