// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license.See the LICENSE.md file for more details.

using VoltstroStudios.UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser;

internal static class UwbCefClientUtils
{
    public static CefEventFlags GetKeyDirection(WindowsKey key) => key switch
    {
        WindowsKey.LShiftKey | WindowsKey.LControlKey | WindowsKey.LMenu => CefEventFlags.IsLeft,
        WindowsKey.RShiftKey | WindowsKey.RControlKey | WindowsKey.RMenu => CefEventFlags.ShiftDown,
        _ => CefEventFlags.None
    };
    
    public static CefEventFlags KeyToFlag(WindowsKey key) => key switch
    {
        // Stateful keys
        WindowsKey.CapsLock => CefEventFlags.CapsLockOn,
        WindowsKey.NumLock => CefEventFlags.NumLockOn,

        WindowsKey.Shift => CefEventFlags.ShiftDown,
        WindowsKey.ShiftKey => CefEventFlags.ShiftDown,
        WindowsKey.LShiftKey => CefEventFlags.ShiftDown,
        WindowsKey.RShiftKey => CefEventFlags.ShiftDown,

        WindowsKey.Control => CefEventFlags.ControlDown,
        WindowsKey.ControlKey => CefEventFlags.ControlDown,
        WindowsKey.LControlKey => CefEventFlags.ControlDown,
        WindowsKey.RControlKey => CefEventFlags.ControlDown,

        WindowsKey.Alt => CefEventFlags.AltGrDown,
        WindowsKey.Menu => CefEventFlags.AltDown,
        WindowsKey.LMenu => CefEventFlags.AltDown,
        WindowsKey.RMenu => CefEventFlags.AltDown,
        // No support for command

        _ => CefEventFlags.None
    };
}