// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Shared.ReadWriters;

internal sealed class MouseScrollEventTypeReadWriter : TypeReadWriter<MouseScrollEvent>
{
    public override void Write(BufferedWriter writer, MouseScrollEvent obj)
    {
        MouseScrollEvent mouseScroll = obj;
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