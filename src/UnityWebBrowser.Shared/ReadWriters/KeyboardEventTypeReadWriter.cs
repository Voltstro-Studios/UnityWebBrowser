using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters
{
    public sealed class KeyboardEventTypeReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            KeyboardEvent keyboardEvent = (KeyboardEvent) obj;
            writer.WriteString(keyboardEvent.Chars);
            WriteKeys(writer, keyboardEvent.KeysDown);
            WriteKeys(writer, keyboardEvent.KeysUp);
        }

        public object Read(BufferedReader reader)
        {
            return new KeyboardEvent
            {
                Chars = reader.ReadString(),
                KeysDown = ReadKeys(reader),
                KeysUp = ReadKeys(reader)
            };
        }

        private void WriteKeys(BufferedWriter writer, int[] keys)
        {
            writer.WriteInt(keys.Length);

            foreach (int key in keys)
            {
                writer.WriteInt(key);
            }
        }

        private int[] ReadKeys(BufferedReader reader)
        {
            int[] keys = new int[reader.ReadInt()];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = reader.ReadInt();
            }

            return keys;
        }
    }
}