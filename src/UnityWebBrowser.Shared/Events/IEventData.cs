using MessagePack;

namespace UnityWebBrowser.Shared.Events
{
    [MessagePackObject]
    [Union(0, typeof(PingEvent))]
    [Union(1, typeof(ShutdownEvent))]
    [Union(2, typeof(KeyboardEvent))]
    [Union(3, typeof(MouseMoveEvent))]
    [Union(4, typeof(MouseClickEvent))]
    [Union(5, typeof(MouseScrollEvent))]
    [Union(6, typeof(NavigateUrlEvent))]
    [Union(7, typeof(GoForwardEvent))]
    [Union(8, typeof(GoBackEvent))]
    [Union(9, typeof(RefreshEvent))]
    [Union(10, typeof(LoadHtmlEvent))]
    [Union(11, typeof(ExecuteJsEvent))]
    public abstract class EventData
    {
    }
}