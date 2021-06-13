using MessagePack;
using MessagePack.Resolvers;
using UnityWebBrowser.Shared.Resolvers;

namespace UnityWebBrowser.Shared
{
    public static class EventsSerializer
    {
        private static readonly MessagePackSerializerOptions Options;
        
        static EventsSerializer()
        {
            Options ??= MessagePackSerializerOptions.Standard.WithResolver(
                CompositeResolver.Create(
                UnityWebBrowserResolver.Instance,
                StandardResolver.Instance));
        }
        
        public static byte[] SerializeEvent<T>(T eventData)
        {
            return MessagePackSerializer.Serialize(eventData, Options);
        }

        public static T DeserializeEvent<T>(byte[] data)
        {
            return MessagePackSerializer.Deserialize<T>(data);
        }
    }
}