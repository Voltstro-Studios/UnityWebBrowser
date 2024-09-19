// UnityWebBrowser (UWB)
// Copyright (c) 2021-2024 Voltstro-Studios
// 
// This project is under the MIT license.See the LICENSE.md file for more details.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace UnityWebBrowser.Engine.Cef.Shared.Core;

#if MACOS

public static class CefMacOsFrameworkLoader
{
    public static void AddFrameworkLoader()
    {
        AssemblyLoadContext.Default.ResolvingUnmanagedDll += (_, libName) =>
        {
            string basePath = Environment.CurrentDirectory;
            bool includeExt = true;
            
            if(libName == "libcef")
            {
                basePath = Path.Combine(basePath, "../Frameworks/", "Chromium Embedded Framework.framework");
                libName = "Chromium Embedded Framework";
                includeExt = false;
            }
            else
            {
                basePath = Path.Combine(basePath, "../Frameworks/", "Chromium Embedded Framework.framework/Libraries");
            }

            string fullPath = Path.GetFullPath(Path.Combine(basePath, libName));
            if (includeExt)
                fullPath += ".dylib";

            return NativeLibrary.Load(fullPath);
        };
    }
}

#endif