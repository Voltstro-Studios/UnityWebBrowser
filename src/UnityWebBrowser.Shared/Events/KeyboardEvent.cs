namespace UnityWebBrowser.Shared.Events
{
    public struct KeyboardEvent
    {
        public int[] KeysUp { get; set; }
        public int[] KeysDown { get; set; }
        public string Chars { get; set; }
    }
}