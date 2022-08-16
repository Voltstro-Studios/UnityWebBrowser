// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Shared.Events;

/// <summary>
///     We use a custom event for pixels to not use the byte[] array type reader/writer
/// </summary>
public struct PixelsEvent
{
    /// <summary>
    ///     The raw pixel data
    /// </summary>
    public ReadOnlyMemory<byte> PixelData { get; set; }
}