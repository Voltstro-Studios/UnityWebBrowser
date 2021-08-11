using System.Collections.Generic;
using UnityEditor;
using UnityWebBrowser.Editor;

namespace UnityWebBrowser.Engine.Cef.Editor
{
    [InitializeOnLoad]
    public static class CefWebBrowser
    {
        private const string CefBrowserEngineWindowsPackageName = "dev.voltstro.unitywebbrowser.engine.cef.win.x64";
        private const string CefBrowserEngineLinuxPackageName = "dev.voltstro.unitywebbrowser.engine.cef.linux.x64";

        static CefWebBrowser()
        {
            Dictionary<BuildTarget, string> buildFiles = new Dictionary<BuildTarget, string>
            {
                { BuildTarget.StandaloneWindows64, $"Packages/{CefBrowserEngineWindowsPackageName}/Plugins/win-x64/" },
                { BuildTarget.StandaloneLinux64, $"Packages/{CefBrowserEngineLinuxPackageName}/Plugins/linux-x64/" }
            };

            BrowserEngineManager.AddBrowserEngine(new BrowserEngine("Cef Browser Engine", "UnityWebBrowser.Engine.Cef",
                buildFiles));
        }
    }
}