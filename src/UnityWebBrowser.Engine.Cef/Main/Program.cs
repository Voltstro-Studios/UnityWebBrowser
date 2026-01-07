// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Runtime.InteropServices;
using UnityWebBrowser.Engine.Cef.Core;

namespace UnityWebBrowser.Engine.Cef;

/// <summary>
///     Main class for this program
/// </summary>
public static class Program
{
    
#if WINDOWS
    //Windows we will export this, and the CEF bootstrap app will run this application
    [UnmanagedCallersOnly(EntryPoint = "RunConsoleMain")]
    public static unsafe int RunConsoleMain(int argc, char** argv, IntPtr sandbox_info)
    {
        //Convert argv to string array
        string[] args = new string[argc];
        for (int i = 0; i < argc; i++)
        {
            string str = Marshal.PtrToStringAnsi((IntPtr)argv[i]);
            args[i] = str;
        }
        
        UwbCefEngineEntry cefEngineEntry = new(sandbox_info);
        return cefEngineEntry.Main(args);
    }
#else
    public static int Main(string[] args)
    {
        UwbCefEngineEntry cefEngineEntry = new(IntPtr.Zero);
        return cefEngineEntry.Main(args);
    }
#endif
    
    
}