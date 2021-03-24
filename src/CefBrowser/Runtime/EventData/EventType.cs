namespace UnityWebBrowser.EventData
{
#if !BROWSER_PROCESS
	[UnityEngine.Scripting.Preserve]
#endif
    public enum EventType
    {
        Ping = 1,
        Shutdown = 2,
        KeyboardEvent = 3,
        MouseMoveEvent = 4,
        MouseClickEvent = 5,
        MouseScrollEvent = 6,
        ButtonEvent = 7,
        LoadHtmlEvent = 8,
        ExecuteJsEvent = 9
    }
}