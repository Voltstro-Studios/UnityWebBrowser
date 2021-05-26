namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    public class KeyboardEvent : IEventData
    {
        public int[] KeysUp { get; set; }
        public int[] KeysDown { get; set; }

        public string Chars { get; set; }
        public EventType EventType { get; set; } = EventType.KeyboardEvent;
    }
}