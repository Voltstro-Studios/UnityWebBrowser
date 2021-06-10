using System.Collections.Generic;
using UnityEditor;
using UnityWebBrowser.Editor;

namespace UnityWebBrowser.Engine.Cef.Editor
{
    [InitializeOnLoad]
    public static class CefWebBrowser
    {
        private const string CefBrowserPackageName = "dev.voltstro.unitywebbrowser";
        
        static CefWebBrowser()
        {
            Dictionary<BuildTarget, string> buildFiles = new Dictionary<BuildTarget, string>
            {
                {BuildTarget.StandaloneWindows64, $"Packages/{CefBrowserPackageName}/Plugins/CefBrowser/win-x64/"},
                {BuildTarget.StandaloneLinux64, $"Packages/{CefBrowserPackageName}/Plugins/CefBrowser/linux-x64/"}
            };

            BrowserEngineManager.AddBrowserEngine(new BrowserEngine("Cef Browser Engine", "UnityWebBrowser.Engine.Cef", buildFiles));
        }
    }
}
