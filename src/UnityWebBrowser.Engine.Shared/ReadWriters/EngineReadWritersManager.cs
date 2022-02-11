using UnityWebBrowser.Shared.ReadWriters;
using VoltRpc.Types;

namespace UnityWebBrowser.Engine.Shared.ReadWriters;

public static class EngineReadWritersManager
{
    public static void AddTypeReadWriters(TypeReaderWriterManager readerWriterManager)
    {
        ReadWriterUtils.AddBaseTypeReadWriters(readerWriterManager);
        readerWriterManager.AddType(new PixelsEventTypeReadWriter());
    }
}