using UnityEngine.Scripting;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [Preserve]
#endif
    public interface IEventData
    {
        public EventType EventType { get; set; }
    }
}