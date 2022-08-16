using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;

internal sealed class MouseMoveEventTypeReadWriter : TypeReadWriter<MouseMoveEvent>
{
    public override void Write(BufferedWriter writer, MouseMoveEvent mouseMove)
    {
        writer.WriteInt(mouseMove.MouseX);
        writer.WriteInt(mouseMove.MouseY);
    }

    public override MouseMoveEvent Read(BufferedReader reader)
    {
        return new MouseMoveEvent
        {
            MouseX = reader.ReadInt(),
            MouseY = reader.ReadInt()
        };
    }
}