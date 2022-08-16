// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Shared;

/// <summary>
///     Screen resolution
/// </summary>
[Serializable]
public struct Resolution
{
    /// <summary>
    ///     Creates a new <see cref="Resolution" /> instance
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public Resolution(uint width, uint height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Width of the screen
    /// </summary>
    public uint Width;

    /// <summary>
    ///     Height of the screen
    /// </summary>
    public uint Height;

    /// <inheritdoc />
    public override string ToString() => $"{Width} x {Height}";

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is Resolution resolution)
        {
            Equals(resolution);
        }

        return false;
    }

    /// <summary>
    ///     Does a different <see cref="Resolution"/> equal to this one?
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Resolution other)
    {
        return Width == other.Width && Height == other.Height;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }
}