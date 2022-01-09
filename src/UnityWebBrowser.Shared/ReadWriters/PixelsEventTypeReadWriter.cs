using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

public class PixelsEventTypeReadWriter : TypeReadWriter<PixelsEvent>
{
    public override void Write(BufferedWriter writer, PixelsEvent pixelsEvent)
    {
        if (pixelsEvent.PixelData == null)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(pixelsEvent.PixelData.Length);

        foreach (byte b in pixelsEvent.PixelData)
            writer.WriteByte(b);
    }

    public override PixelsEvent Read(BufferedReader reader)
    {
        PixelsEvent pixelsEvent = new();
        int size = reader.ReadInt();
        if (size == -1)
            return pixelsEvent;

        pixelsEvent.PixelData = new byte[size];
        for (int i = 0; i < size; i++) pixelsEvent.PixelData[i] = reader.ReadByte();

        return pixelsEvent;
    }
}