// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using UnityWebBrowser.Engine.Cef.Core;

namespace UnityWebBrowser.Engine.Cef;

/// <summary>
///     Main class for this program
/// </summary>
public static class Program
{
    /// <summary>
    ///     Entry point
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static int Main(string[] args)
    {
        UwbCefEngineEntry cefEngineEntry = new();
        return cefEngineEntry.Main(args);
    }
}