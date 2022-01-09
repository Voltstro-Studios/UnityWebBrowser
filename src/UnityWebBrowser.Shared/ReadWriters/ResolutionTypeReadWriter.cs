using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

public class ResolutionTypeReadWriter : TypeReadWriter<Resolution>
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