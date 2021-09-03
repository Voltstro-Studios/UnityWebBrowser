using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters
{
    public class ResolutionTypeReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            Resolution resolution = (Resolution)obj;
            writer.WriteUInt(resolution.Width);
            writer.WriteUInt(resolution.Height);
        }

        public object Read(BufferedReader reader)
        {
            return new Resolution(reader.ReadUInt(), reader.ReadUInt());
        }
    }
}