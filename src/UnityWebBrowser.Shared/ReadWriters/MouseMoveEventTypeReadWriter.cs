using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters
{
    public sealed class MouseMoveEventTypeReadWriter : TypeReadWriter<MouseMoveEvent>
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
}