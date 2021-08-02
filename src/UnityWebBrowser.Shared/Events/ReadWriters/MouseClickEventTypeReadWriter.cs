using UnityWebBrowser.Shared.Events.EngineAction;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.Events.ReadWriters
{
    public sealed class MouseClickEventTypeReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            MouseClickEvent mouseClickEvent = (MouseClickEvent) obj;
            writer.WriteInt(mouseClickEvent.MouseX);
            writer.WriteInt(mouseClickEvent.MouseY);
            writer.WriteInt(mouseClickEvent.MouseClickCount);
            writer.WriteByte((byte)mouseClickEvent.MouseClickType);
            writer.WriteByte((byte)mouseClickEvent.MouseEventType);
        }

        public object Read(BufferedReader reader)
        {
            return new MouseClickEvent
            {
                MouseX = reader.ReadInt(),
                MouseY = reader.ReadInt(),
                MouseClickCount = reader.ReadInt(),
                MouseClickType = (MouseClickType) reader.ReadByte(),
                MouseEventType = (MouseEventType) reader.ReadByte()
            };
        }
    }
}
