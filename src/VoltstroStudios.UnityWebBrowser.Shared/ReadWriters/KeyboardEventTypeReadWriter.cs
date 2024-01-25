// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;

internal sealed class KeyboardEventTypeReadWriter : TypeReadWriter<KeyboardEvent>
{
    public override void Write(BufferedWriter writer, KeyboardEvent keyboardEvent)
    {
        WriteChars(writer, keyboardEvent.Chars);
        WriteKeys(writer, keyboardEvent.KeysDown);
        WriteKeys(writer, keyboardEvent.KeysUp);
    }

    public override KeyboardEvent Read(BufferedReader reader)
    {
        return new KeyboardEvent
        {
            Chars = ReadChars(reader),
            KeysDown = ReadKeys(reader),
            KeysUp = ReadKeys(reader)
        };
    }

    private void WriteChars(BufferedWriter writer, char[] chars)
    {
        writer.WriteInt(chars.Length);
        for (int i = 0; i < chars.Length; i++)
        {
            writer.WriteChar(chars[i]);
        }
    }

    private char[] ReadChars(BufferedReader reader)
    {
        char[] chars = new char[reader.ReadInt()];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = reader.ReadChar();
        }

        return chars;
    }

    private void WriteKeys(BufferedWriter writer, WindowsKey[] keys)
    {
        writer.WriteInt(keys.Length);

        foreach (WindowsKey key in keys) writer.WriteInt((int) key);
    }

    private WindowsKey[] ReadKeys(BufferedReader reader)
    {
        WindowsKey[] keys = new WindowsKey[reader.ReadInt()];
        for (int i = 0; i < keys.Length; i++) keys[i] = (WindowsKey) reader.ReadInt();

        return keys;
    }
}