using MessagePack;

namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
    [UnityEngine.Scripting.Preserve]
#endif
    [MessagePackObject]
    public class ExecuteJsEvent : IEventData
    {
        [Key(0)]
        public string Js { get; set; }
    }
}