using System;
using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters
{
    public class PixelsEventTypeReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            PixelsEvent pixelsEvent = (PixelsEvent)obj;
            if (pixelsEvent.PixelData == null)
            {
                writer.WriteInt(-1);
                return;
            }
            
            writer.WriteInt(pixelsEvent.PixelData.Length);
            
            foreach (byte b in pixelsEvent.PixelData)
                writer.WriteByte(b);
        }

        public object Read(BufferedReader reader)
        {
            PixelsEvent pixelsEvent = new PixelsEvent();
            int size = reader.ReadInt();
            if (size == -1) 
                return pixelsEvent;

            pixelsEvent.PixelData = new byte[size];
            for (int i = 0; i < size; i++) pixelsEvent.PixelData[i] = reader.ReadByte();
            
            return pixelsEvent;
        }
    }
}