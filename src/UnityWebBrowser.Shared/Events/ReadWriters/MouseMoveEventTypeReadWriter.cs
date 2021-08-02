using UnityWebBrowser.Shared.Events.EngineAction;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.Events.ReadWriters
{
    public sealed class MouseMoveEventTypeReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            MouseMoveEvent mouseMove = (MouseMoveEvent) obj;
            writer.WriteInt(mouseMove.MouseX);
            writer.WriteInt(mouseMove.MouseY);
        }

        public object Read(BufferedReader reader)
        {
            return new MouseMoveEvent
            {
                MouseX = reader.ReadInt(),
                MouseY = reader.ReadInt()
            };
        }
    }
}