using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    [MessagePackObject]
    public class ExecuteJsEvent : EventData
    {
        [Key(0)]
        public string Js { get; set; }
    }
}