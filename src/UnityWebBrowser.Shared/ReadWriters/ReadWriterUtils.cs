using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

internal static class ReadWriterUtils
{
    public static void AddBaseTypeReadWriters(TypeReaderWriterManager readerWriterManager)
    {
        readerWriterManager.AddType(new KeyboardEventTypeReadWriter());
        readerWriterManager.AddType(new MouseClickEventTypeReadWriter());
        readerWriterManager.AddType(new MouseMoveEventTypeReadWriter());
        readerWriterManager.AddType(new MouseScrollEventTypeReadWriter());
        readerWriterManager.AddType(new ResolutionTypeReadWriter());
        
        //TODO: Won't need this when we push these types to VoltRpc
        readerWriterManager.AddType(new Vector2TypeReadWriter());
        readerWriterManager.AddType(new GuidTypeReaderWriter());
    }
}