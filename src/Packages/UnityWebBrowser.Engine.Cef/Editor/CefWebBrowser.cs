using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityWebBrowser.Editor.EngineManagement;

namespace UnityWebBrowser.Engine.Cef.Editor
{
    [InitializeOnLoad]
    public static class CefWebBrowser
    {
        private const string CefBrowserEngineWindowsPackageName = "dev.voltstro.unitywebbrowser.engine.cef.win.x64";
        private const string CefBrowserEngineLinuxPackageName = "dev.voltstro.unitywebbrowser.engine.cef.linux.x64";

        static CefWebBrowser()
        {
            Dictionary<BuildTarget, string> buildFiles = new();

            string windowsPath = $"Packages/{CefBrowserEngineWindowsPackageName}/Engine~/";
            string linuxPath = $"Packages/{CefBrowserEngineLinuxPackageName}/Engine~/";

            if (Directory.Exists(windowsPath))
                buildFiles.Add(BuildTarget.StandaloneWindows64, windowsPath);
            if (Directory.Exists(linuxPath))
                buildFiles.Add(BuildTarget.StandaloneLinux64, linuxPath);

            if (buildFiles.Count > 0)
                BrowserEngineManager.AddEngine(new BrowserEngine("Cef Engine", "UnityWebBrowser.Engine.Cef",
                    buildFiles));
            else
                Debug.LogWarning($"The base Cef Engine has been installed, but no platform packages have been added! You need to add either {CefBrowserEngineWindowsPackageName} or {CefBrowserEngineLinuxPackageName} to your project!");
        }
    }
}