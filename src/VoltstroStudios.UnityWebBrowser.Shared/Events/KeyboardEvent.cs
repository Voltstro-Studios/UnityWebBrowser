// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

namespace VoltstroStudios.UnityWebBrowser.Shared.Events;

/// <summary>
///     Event related to the keyboard
/// </summary>
public struct KeyboardEvent
{
    /// <summary>
    ///     The keys that were released this frame
    /// </summary>
    public WindowsKey[] KeysUp { get; set; }

    /// <summary>
    ///     The keys that were pressed down this frame
    /// </summary>
    public WindowsKey[] KeysDown { get; set; }

    /// <summary>
    ///     The characters that were pressed this frame
    /// </summary>
    public char[] Chars { get; set; }
}