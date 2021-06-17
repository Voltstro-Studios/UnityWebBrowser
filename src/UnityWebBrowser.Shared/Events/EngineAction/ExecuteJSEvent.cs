using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineAction
{
    [MessagePackObject]
    public class ExecuteJsEvent : EngineActionEvent
    {
        [Key(0)]
        public string Js { get; set; }
    }
}