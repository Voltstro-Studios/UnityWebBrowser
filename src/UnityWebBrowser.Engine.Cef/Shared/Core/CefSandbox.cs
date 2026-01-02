// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license.See the LICENSE.md file for more details.

using System;
using System.Runtime.InteropServices;

namespace UnityWebBrowser.Engine.Cef.Shared.Core;

/// <summary>
///     CEF Sandbox methods
/// </summary>
public static class CefSandbox
{
#if MACOS
    [DllImport("libcef_sandbox", EntryPoint = "cef_sandbox_initialize", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr cef_sandbox_initialize(int argc, [In] string[] argv);

    [DllImport("libcef_sandbox", EntryPoint = "cef_sandbox_destroy", CallingConvention = CallingConvention.Cdecl)]
    public static extern void cef_sandbox_destroy(IntPtr sandbox_context);

#endif
}