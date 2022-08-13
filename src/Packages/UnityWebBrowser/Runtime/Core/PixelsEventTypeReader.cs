using System;
using Unity.Collections;
using UnityWebBrowser.Shared.Events;
using VoltRpc.Extension.Memory;
using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.NativeArraySpanExtensions;

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
            
            ReadOnlySpan<byte> data = reader.ReadBytesSpanSlice(size);
            if (!pixelData.IsCreated || pixelData.Length != size)
                return default;
            
            pixelData.CopyFrom(data);

            return default;
        }
    }
}