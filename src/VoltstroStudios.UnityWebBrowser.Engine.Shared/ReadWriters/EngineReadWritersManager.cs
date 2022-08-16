// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;
using VoltRpc.Types;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.ReadWriters;

public static class EngineReadWritersManager
{
    public static void AddTypeReadWriters(TypeReaderWriterManager readerWriterManager)
    {
        ReadWriterUtils.AddBaseTypeReadWriters(readerWriterManager);
        readerWriterManager.AddType(new PixelsEventTypeReadWriter());
    }
}