#nullable enable
using System;
using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

public class PixelsEventTypeReadWriter : TypeReadWriter<PixelsEvent>
{
    private byte[]? buffer;

    public override void Write(BufferedWriter writer, PixelsEvent pixelsEvent)
    {
        if (pixelsEvent.PixelData.Length <= 0)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(pixelsEvent.PixelData.Length);

        foreach (byte b in pixelsEvent.PixelData.Span)
            writer.WriteByte(b);
    }

    public override PixelsEvent Read(BufferedReader reader)
    {
        PixelsEvent pixelsEvent = new();
        int size = reader.ReadInt();
        if (size == -1)
            return pixelsEvent;

        if (buffer == null || buffer.Length < size)
            buffer = new byte[size];
        
        for (int i = 0; i < size; i++) 
            buffer[i] = reader.ReadByte();

        pixelsEvent.PixelData = new ReadOnlyMemory<byte>(buffer, 0, size);

        return pixelsEvent;
    }
}