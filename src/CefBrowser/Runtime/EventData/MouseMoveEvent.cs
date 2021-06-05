using MessagePack;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    [MessagePackObject]
    public class MouseMoveEvent : IEventData
    {
        [Key(0)]
        public int MouseX { get; set; }

        [Key(1)]
        public int MouseY { get; set; }
    }
}