using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineActions
{
    [MessagePackObject]
    public class MouseMoveEvent : EngineActionEvent
    {
        [Key(0)]
        public int MouseX { get; set; }

        [Key(1)]
        public int MouseY { get; set; }
    }
}