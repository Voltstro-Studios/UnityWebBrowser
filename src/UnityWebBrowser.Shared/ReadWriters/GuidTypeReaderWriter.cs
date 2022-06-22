using System;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

//TODO: Merge into VoltRpc
public class GuidTypeReaderWriter : TypeReadWriter<Guid>
{
    public override void Write(BufferedWriter writer, Guid value)
    {
        byte[] data = value.ToByteArray();
        for (int i = 0; i < data.Length; i++)
        {
            writer.WriteByte(data[i]);
        }
    }

    public override Guid Read(BufferedReader reader)
    {
        byte[] data = new byte[16];
        for (int i = 0; i < 16; i++)
        {
            data[i] = reader.ReadByte();
        }
        
        Guid guid = new Guid(data);
        return guid;
    }
}