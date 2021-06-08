using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    [MessagePackObject]
    public class KeyboardEvent : IEventData
    {
        [Key(0)]
        public int[] KeysUp { get; set; }

        [Key(1)]
        public int[] KeysDown { get; set; }

        [Key(2)]
        public string Chars { get; set; }
    }
}