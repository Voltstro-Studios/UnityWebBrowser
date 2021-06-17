using MessagePack;

namespace UnityWebBrowser.Shared.Events.EngineActionResponse
{
    [MessagePackObject]
    public class PixelsResponse : EngineActionResponse
    {
        [Key(0)] public byte[] Pixels { get; set; }
    }
}