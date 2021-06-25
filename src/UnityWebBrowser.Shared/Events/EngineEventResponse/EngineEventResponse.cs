using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineEventResponse
{
    [MessagePackObject]
    [Union(0, typeof(OkEngineEventResponse))]
    public abstract class EngineEventResponse
    {
    }
}