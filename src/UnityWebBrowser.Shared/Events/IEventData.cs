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
    [Union(6, typeof(ButtonEvent))]
    [Union(7, typeof(LoadHtmlEvent))]
    [Union(8, typeof(ExecuteJsEvent))]
    public abstract class IEventData
    {
    }
}