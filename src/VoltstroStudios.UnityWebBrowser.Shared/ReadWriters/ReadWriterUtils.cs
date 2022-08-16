using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Types;

namespace VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;

internal static class ReadWriterUtils
{
    public static void AddBaseTypeReadWriters(TypeReaderWriterManager readerWriterManager)
    {
        readerWriterManager.AddType(new KeyboardEventTypeReadWriter());
        readerWriterManager.AddType(new MouseClickEventTypeReadWriter());
        readerWriterManager.AddType(new MouseMoveEventTypeReadWriter());
        readerWriterManager.AddType(new MouseScrollEventTypeReadWriter());
        readerWriterManager.AddType(new ResolutionTypeReadWriter());
        readerWriterManager.InstallVectorsExtension();
    }
}