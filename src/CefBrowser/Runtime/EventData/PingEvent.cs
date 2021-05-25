using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [Preserve]
#endif
    public class PingEvent : IEventData
    {
        public EventType EventType { get; set; } = EventType.Ping;
    }
}