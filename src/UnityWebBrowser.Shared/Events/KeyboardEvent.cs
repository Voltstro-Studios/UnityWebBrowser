namespace UnityWebBrowser.Shared.Events;

public struct KeyboardEvent
{
    public WindowsKey[] KeysUp { get; set; }
    public WindowsKey[] KeysDown { get; set; }
    public string Chars { get; set; }
}