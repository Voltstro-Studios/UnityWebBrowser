using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;

internal sealed class MouseClickEventTypeReadWriter : TypeReadWriter<MouseClickEvent>
{
    public override void Write(BufferedWriter writer, MouseClickEvent mouseClickEvent)
    {
        writer.WriteInt(mouseClickEvent.MouseX);
        writer.WriteInt(mouseClickEvent.MouseY);
        writer.WriteInt(mouseClickEvent.MouseClickCount);
        writer.WriteByte((byte) mouseClickEvent.MouseClickType);
        writer.WriteByte((byte) mouseClickEvent.MouseEventType);
    }

    public override MouseClickEvent Read(BufferedReader reader)
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