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
#if WINDOWS
    /// <summary>
    /// Create the sandbox information object for this process. It is safe to create
    /// multiple of this object and to destroy the object immediately after passing
    /// into the CefExecuteProcess() and/or CefInitialize() functions.
    /// </summary>
    /// <returns></returns>
    [DllImport("cef_sandbox", EntryPoint = "cef_sandbox_info_create", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr cef_sandbox_info_create();

    /// <summary>
    /// Destroy the specified sandbox information object.
    /// </summary>
    /// <param name="sandbox_info"></param>
    /// <returns></returns>
    [DllImport("cef_sandbox", EntryPoint = "cef_sandbox_info_destroy", CallingConvention = CallingConvention.Cdecl)]
    public static extern void cef_sandbox_info_destroy(IntPtr sandbox_info);
#endif

#if MACOS
    [DllImport("cef_sandbox", EntryPoint = "cef_sandbox_initialize", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr cef_sandbox_initialize(int argc, [In] string[] argv);

    [DllImport("cef_sandbox", EntryPoint = "cef_sandbox_destroy", CallingConvention = CallingConvention.Cdecl)]
    public static extern void cef_sandbox_destroy(IntPtr sandbox_context);

#endif
}