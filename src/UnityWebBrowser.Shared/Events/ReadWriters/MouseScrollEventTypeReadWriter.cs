using UnityWebBrowser.Shared.Events.EngineAction;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.Events.ReadWriters
{
    public sealed class MouseScrollEventTypeReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            MouseScrollEvent mouseScroll = (MouseScrollEvent) obj;
            writer.WriteInt(mouseScroll.MouseX);
            writer.WriteInt(mouseScroll.MouseY);
            writer.WriteInt(mouseScroll.MouseScroll);
        }

        public object Read(BufferedReader reader)
        {
            return new MouseScrollEvent
            {
                MouseX = reader.ReadInt(),
                MouseY = reader.ReadInt(),
                MouseScroll = reader.ReadInt()
            };
        }
    }
}