using System.IO;
using UnityEngine;

namespace UnityWebBrowser
{
    public static class WebBrowserUtils
    {
	    public const string PackageName = "dev.voltstro.unitywebbrowser";

	    public static string GetCefProcessPath()
	    {
#if UNITY_EDITOR
		    return Path.GetFullPath($"Packages/{PackageName}/Plugins/CefBrowser/CefBrowserProcess.exe");
#else
			return Path.GetFullPath($"{Application.dataPath}/Plugins/x86_64/CefBrowserProcess.exe");
#endif
	    }
    }
}