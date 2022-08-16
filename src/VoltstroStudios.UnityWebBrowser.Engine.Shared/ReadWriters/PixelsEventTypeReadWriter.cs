// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

#nullable enable
using System;
using VoltstroStudios.UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.ReadWriters;

public class PixelsEventTypeReadWriter : TypeReadWriter<PixelsEvent>
{
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
        throw new NotImplementedException();
    }
}