// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;

internal class ResolutionTypeReadWriter : TypeReadWriter<Resolution>
{
    public override void Write(BufferedWriter writer, Resolution resolution)
    {
        writer.WriteUInt(resolution.Width);
        writer.WriteUInt(resolution.Height);
    }

    public override Resolution Read(BufferedReader reader)
    {
        return new Resolution(reader.ReadUInt(), reader.ReadUInt());
    }
}