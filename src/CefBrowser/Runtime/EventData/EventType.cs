#if BROWSER_PROCESS
namespace CefBrowserProcess.EventData
#else
namespace UnityWebBrowser.EventData
#endif
{
    public enum EventType
    {
        Ping = 1,
        Shutdown = 2,
        KeyboardEvent = 3,
        MouseMoveEvent = 4,
        MouseClickEvent = 5,
        MouseScrollEvent = 6,
        ButtonEvent = 7
    }
}