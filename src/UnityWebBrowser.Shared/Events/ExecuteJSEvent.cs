using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    [MessagePackObject]
    public class ExecuteJsEvent : IEventData
    {
        [Key(0)]
        public string Js { get; set; }
    }
}