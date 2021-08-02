using UnityWebBrowser.Shared.Events.EngineAction;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.Events.ReadWriters
{
    public static class ReadWriterUtils
    {
        public static void AddTypeReadWriters(TypeReaderWriterManager readerWriterManager)
        {
            readerWriterManager.AddType<KeyboardEvent>(new KeyboardEventTypeReadWriter());
            readerWriterManager.AddType<MouseClickEvent>(new MouseClickEventTypeReadWriter());
            readerWriterManager.AddType<MouseMoveEvent>(new MouseMoveEventTypeReadWriter());
            readerWriterManager.AddType<MouseScrollEvent>(new MouseScrollEventTypeReadWriter());
        }
    }
}