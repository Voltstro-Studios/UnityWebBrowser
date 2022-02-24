using System;
using Unity.Collections;
using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Core
{
    internal class PixelsEventTypeReader : TypeReadWriter<PixelsEvent>
    {
        private NativeArray<byte> pixelData;

        public PixelsEventTypeReader(NativeArray<byte> textureData)
        {
            SetPixelDataArray(textureData);
        }
        
        public void SetPixelDataArray(NativeArray<byte> array)
        {
            pixelData = array;
        }

        public override void Write(BufferedWriter writer, PixelsEvent value)
        {
            throw new NotImplementedException();
        }

        public override PixelsEvent Read(BufferedReader reader)
        {
            //Read the size first
            int size = reader.ReadInt();
            if (size <= 0)
                return default;
            
            if (!pixelData.IsCreated || pixelData.Length != size)
            {
                for (int i = 0; i < size; i++)
                {
                    reader.ReadByte();
                }
            }

            for (int i = 0; i < size; i++)
            {
                pixelData[i] = reader.ReadByte();
            }

            return default;
        }
    }
}