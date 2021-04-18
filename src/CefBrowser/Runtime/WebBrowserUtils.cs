using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace UnityWebBrowser
{
	/// <summary>
	///		Provides Utils to be used by the web browser
	/// </summary>
    public static class WebBrowserUtils
    {
#if UNITY_EDITOR
	    /// <summary>
		///		The name of the package that the web browser is contained in
		/// </summary>
	    public const string PackageName = "dev.voltstro.unitywebbrowser";
#endif

		/// <summary>
		///		Gets the main directory where logs and cache is stored
		/// </summary>
		/// <returns></returns>
	    public static string GetCefMainDirectory()
	    {
#if UNITY_EDITOR
		    return Path.GetFullPath($"{Directory.GetParent(Application.dataPath).FullName}/Library");
#else
			return Application.dataPath;
#endif
	    }

		/// <summary>
		///		Gets the folder that the cef process application lives in
		/// </summary>
		/// <returns></returns>
	    public static string GetCefProcessPath()
	    {
#if UNITY_EDITOR_LINUX
		    return Path.GetFullPath($"Packages/{PackageName}/Plugins/CefBrowser/linux-x64/");
#elif UNITY_EDITOR_WIN
		    return Path.GetFullPath($"Packages/{PackageName}/Plugins/CefBrowser/win-x64/");
#elif UNITY_STANDALONE_WIN
		    return Path.GetFullPath($"{Application.dataPath}/Plugins/x86_64/");
#elif UNITY_STANDALONE_LINUX
		    return Path.GetFullPath($"{Application.dataPath}/Plugins/");
#endif
	    }

		/// <summary>
		///		Get a direct path to the cef browser process application
		/// </summary>
		/// <returns></returns>
	    public static string GetCefProcessApplication()
	    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		    return $"{GetCefProcessPath()}/CefBrowserProcess.exe";
#else
		    return $"{GetCefProcessPath()}/CefBrowserProcess";
#endif
	    }

		/// <summary>
		///		Gets the local position delta (0 -> 1) from a screen position on a <see cref="RawImage"/>
		///		from a top-left origin point
		///		<para>To calculate the pixel position,
		///		do <see cref="Vector2.x"/> * [Desired height] and <see cref="Vector2.y"/> * [Desired Width]</para>
		/// </summary>
		/// <param name="image"><see cref="RawImage"/> that you want to calculate the local position on</param>
		/// <param name="screenPosition">The screen position</param>
		/// <param name="position">The local delta position</param>
		/// <returns></returns>
	    public static bool GetScreenPointToLocalPositionDeltaOnImage(RawImage image, Vector2 screenPosition, out Vector2 position)
	    {
			//This was a pain in the ass to figure out how to do, I never want anything to do with mouses and UI elements ever again.

			//The main pain was that CEF uses a top left origin point, not center like Unity, but turns out its dog shit
			//simple to do and that I am terrible at maths

			//There probs something here that could be done better, if you know, send in a PR
			//Based off: http://answers.unity.com/answers/1455168/view.html

		    position = new Vector2();
		    RectTransform uiImageObjectRect = image.rectTransform;
		    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(uiImageObjectRect, screenPosition, null,
			    out Vector2 localCursor)) return false;

		    Vector2 ptPivotCancelledLocation = new Vector2(localCursor.x - uiImageObjectRect.rect.x,
			    localCursor.y - uiImageObjectRect.rect.y);
		    Vector2 ptLocationRelativeToImageInScreenCoordinates =
			    new Vector2(ptPivotCancelledLocation.x, ptPivotCancelledLocation.y);
		    position.x = ptLocationRelativeToImageInScreenCoordinates.x / uiImageObjectRect.rect.width;
		    position.y = -(ptLocationRelativeToImageInScreenCoordinates.y / uiImageObjectRect.rect.height) + 1;

		    return true;
	    }
    }
}