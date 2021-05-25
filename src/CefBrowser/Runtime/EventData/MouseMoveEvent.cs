using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [Preserve]
#endif
    public class MouseMoveEvent : IEventData
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public EventType EventType { get; set; } = EventType.MouseMoveEvent;
    }
}