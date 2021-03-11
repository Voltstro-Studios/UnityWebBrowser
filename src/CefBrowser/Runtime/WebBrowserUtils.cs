using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

		/// <summary>
		///		Gets the local position delta (0 -> 1) from a screen position on a <see cref="RawImage"/>
		/// </summary>
		/// <param name="image"><see cref="RawImage"/> that you want to calculate the local position on</param>
		/// <param name="screenPosition">The screen position</param>
		/// <param name="position">The local delta position</param>
		/// <returns></returns>
	    public static bool GetScreenPointToLocalPositionDeltaOnImage(RawImage image, Vector2 screenPosition, out Vector2 position)
	    {
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