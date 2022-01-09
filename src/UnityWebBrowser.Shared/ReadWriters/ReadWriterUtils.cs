using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

public static class ReadWriterUtils
{
    public static void AddTypeReadWriters(TypeReaderWriterManager readerWriterManager)
    {
        readerWriterManager.AddType(new KeyboardEventTypeReadWriter());
        readerWriterManager.AddType(new MouseClickEventTypeReadWriter());
        readerWriterManager.AddType(new MouseMoveEventTypeReadWriter());
        readerWriterManager.AddType(new MouseScrollEventTypeReadWriter());
        readerWriterManager.AddType(new PixelsEventTypeReadWriter());
        readerWriterManager.AddType(new ResolutionTypeReadWriter());
    }
}