// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System.IO;
using System.Reflection;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.Pages;

/// <summary>
///     Handler for our embedded resources of custom pages
/// </summary>
public static class PagesHandler
{
    private static readonly Assembly EngineSharedAssembly = typeof(PagesHandler).Assembly;

    public static Stream GetAboutPageCode()
    {
        Stream stream =
            EngineSharedAssembly.GetManifestResourceStream("UnityWebBrowser.Engine.Shared.Pages.About.html");
        return stream;
    }
}