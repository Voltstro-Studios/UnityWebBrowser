using UnityWebBrowser.Shared.Events;
using VoltRpc.IO;
using VoltRpc.Types;

namespace UnityWebBrowser.Shared.ReadWriters;

public sealed class KeyboardEventTypeReadWriter : TypeReadWriter<KeyboardEvent>
{
    public override void Write(BufferedWriter writer, KeyboardEvent keyboardEvent)
    {
        writer.WriteString(keyboardEvent.Chars);
        WriteKeys(writer, keyboardEvent.KeysDown);
        WriteKeys(writer, keyboardEvent.KeysUp);
    }

    public override KeyboardEvent Read(BufferedReader reader)
    {
        return new KeyboardEvent
        {
            Chars = reader.ReadString(),
            KeysDown = ReadKeys(reader),
            KeysUp = ReadKeys(reader)
        };
    }

    private void WriteKeys(BufferedWriter writer, WindowsKey[] keys)
    {
        writer.WriteInt(keys.Length);

        foreach (WindowsKey key in keys) writer.WriteInt((int)key);
    }

    private WindowsKey[] ReadKeys(BufferedReader reader)
    {
        WindowsKey[] keys = new WindowsKey[reader.ReadInt()];
        for (int i = 0; i < keys.Length; i++) keys[i] = (WindowsKey)reader.ReadInt();

        return keys;
    }
}