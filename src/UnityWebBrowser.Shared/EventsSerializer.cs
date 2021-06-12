using MessagePack;
using MessagePack.Resolvers;
using UnityWebBrowser.Shared.Events;
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
        
        public static byte[] Serialize(EventData eventData)
        {
            return MessagePackSerializer.Serialize(eventData, Options);
        }

        public static EventData Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<EventData>(data);
        }
    }
}