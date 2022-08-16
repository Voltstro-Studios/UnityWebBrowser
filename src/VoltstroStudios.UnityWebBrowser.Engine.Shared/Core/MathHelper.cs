// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     Helper methods for math
/// </summary>
public static class MathHelper
{
    /// <summary>
    ///     Converts a hex to a <see cref="byte" />
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static byte HexToDec(string hex)
    {
        byte dec = Convert.ToByte(hex, 16);
        return dec;
    }
}