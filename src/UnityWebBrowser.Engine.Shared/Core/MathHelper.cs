using System;

namespace UnityWebBrowser.Engine.Shared.Core;

/// <summary>
///     Helper methods for math
/// </summary>
public static class MathHelper
{
    /// <summary>
    ///     Converts a hex to a <see cref="byte"/>
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static byte HexToDec(string hex)
    {
        byte dec = Convert.ToByte(hex, 16);
        return dec;
    }
}