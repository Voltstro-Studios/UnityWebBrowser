using System;

namespace UnityWebBrowser.Shared.Events.EngineAction
{
    [Serializable]
    public class MouseMoveEvent
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }
    }
}