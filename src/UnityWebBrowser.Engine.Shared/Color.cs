using System;
using UnityWebBrowser.Engine.Shared.Core;

namespace UnityWebBrowser.Engine.Shared;

public readonly struct Color
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }

    public Color(string hex)
    {
        if (hex.Length < 6)
            throw new ArgumentOutOfRangeException(nameof(hex));


        R = MathHelper.HexToDec(hex.Substring(0, 2));
        G = MathHelper.HexToDec(hex.Substring(2, 2));
        B = MathHelper.HexToDec(hex.Substring(4, 2));

        A = hex.Length >= 8 ? MathHelper.HexToDec(hex.Substring(6, 2)) : (byte) 255;
    }

    public override string ToString()
    {
        return $"({R}, {G}, {B}, {A})";
    }
}