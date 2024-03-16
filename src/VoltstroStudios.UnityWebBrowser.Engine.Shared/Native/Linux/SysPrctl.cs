// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Native.Linux;

[SupportedOSPlatform("Linux")]
internal static class SysPrctl
{
    /// <summary>
    ///     Operations on a process or thread
    ///     https://www.man7.org/linux/man-pages/man2/prctl.2.html
    /// </summary>
    /// <param name="option"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    [DllImport("libc")]
    public static extern int prctl(int option, ulong arg2);
}