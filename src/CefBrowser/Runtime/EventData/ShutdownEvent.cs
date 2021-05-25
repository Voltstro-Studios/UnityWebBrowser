using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [Preserve]
#endif
    public class ShutdownEvent : IEventData
    {
        public EventType EventType { get; set; } = EventType.Shutdown;
    }
}