// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared;

/// <summary>
///     Container for a color
/// </summary>
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