using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters
{
    public sealed class MouseScrollEventTypeReadWriter : TypeReadWriter<MouseScrollEvent>
    {
        public override void Write(BufferedWriter writer, MouseScrollEvent obj)
        {
            MouseScrollEvent mouseScroll = (MouseScrollEvent)obj;
            writer.WriteInt(mouseScroll.MouseX);
            writer.WriteInt(mouseScroll.MouseY);
            writer.WriteInt(mouseScroll.MouseScroll);
        }

        public override MouseScrollEvent Read(BufferedReader reader)
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