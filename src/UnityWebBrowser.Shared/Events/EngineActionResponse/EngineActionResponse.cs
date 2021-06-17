using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineActionResponse
{
    [MessagePackObject]
    [Union(0, typeof(OkResponse))]
    [Union(1, typeof(PixelsResponse))]
    public abstract class EngineActionResponse
    {
    }
}