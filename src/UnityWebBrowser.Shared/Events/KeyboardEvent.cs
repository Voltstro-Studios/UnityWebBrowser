namespace UnityWebBrowser.Shared.Events
{
    public class KeyboardEvent
    {
        public int[] KeysUp { get; set; }
        public int[] KeysDown { get; set; }
        public string Chars { get; set; }
    }
}