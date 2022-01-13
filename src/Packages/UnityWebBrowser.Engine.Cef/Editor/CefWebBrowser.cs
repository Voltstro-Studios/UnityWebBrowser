using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityWebBrowser.Editor;
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
            Dictionary<BuildTarget, string> buildFiles = new Dictionary<BuildTarget, string>();
            
            string windowsPath = $"Packages/{CefBrowserEngineWindowsPackageName}/Plugins/win-x64/";
            string linuxPath = $"Packages/{CefBrowserEngineLinuxPackageName}/Plugins/linux-x64/";
            
            if(Directory.Exists(windowsPath))
                buildFiles.Add(BuildTarget.StandaloneWindows64, windowsPath);
            if(Directory.Exists(linuxPath))
                buildFiles.Add(BuildTarget.StandaloneLinux64, linuxPath);
            
            if(buildFiles.Count > 0)
                BrowserEngineManager.AddEngine(new BrowserEngine("Cef Browser Engine", "UnityWebBrowser.Engine.Cef",
                buildFiles));
        }
    }
}