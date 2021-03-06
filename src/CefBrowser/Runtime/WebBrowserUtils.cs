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
		    return Path.GetFullPath($"Packages/{PackageName}/Plugins/CefBrowser/");
#else
			return Path.GetFullPath($"{Application.dataPath}/Plugins/x86_64/");
#endif
	    }

	    public static string GetCefProcessApplication()
	    {
		    return $"{GetCefProcessPath()}/CefBrowserProcess.exe";
	    }
    }
}