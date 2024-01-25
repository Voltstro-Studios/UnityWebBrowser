// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

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
    }
}