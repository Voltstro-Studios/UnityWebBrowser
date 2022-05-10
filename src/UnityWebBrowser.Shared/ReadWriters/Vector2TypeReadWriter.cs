using System.Numerics;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

//TODO: Push upstream VoltRpc
internal sealed class Vector2TypeReadWriter : TypeReadWriter<Vector2>
{
    public override void Write(BufferedWriter writer, Vector2 value)
    {
        writer.WriteFloat(value.X);
        writer.WriteFloat(value.Y);
    }

    public override Vector2 Read(BufferedReader reader)
    {
        return new Vector2(reader.ReadFloat(), reader.ReadFloat());
    }
}