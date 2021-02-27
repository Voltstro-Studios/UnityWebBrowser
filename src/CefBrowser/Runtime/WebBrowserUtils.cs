using System.IO;

namespace UnityWebBrowser
{
    public static class WebBrowserUtils
    {
	    public const string PackageName = "dev.voltstro.unitywebbrowser";

	    public static string GetCefProcessPath()
	    {
#if UNITY_EDITOR
		    return Path.GetFullPath($"Packages/{PackageName}/Plugins/CefBrowser/CefBrowserProcess.exe");
#endif
	    }
    }
}