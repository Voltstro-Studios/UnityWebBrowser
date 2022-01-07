using System;

namespace UnityWebBrowser.Engine.Shared.Core;

public static class MathHelper
{
    public static byte HexToDec(string hex)
    {
        byte dec = Convert.ToByte(hex, 16);
        return dec;
    }
}