using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [Preserve]
#endif
    public class MouseScrollEvent : IEventData
    {
        public int MouseX { get; set; }
        public int MouseY { get; set; }

        public int MouseScroll { get; set; }
        public EventType EventType { get; set; } = EventType.MouseScrollEvent;
    }
}