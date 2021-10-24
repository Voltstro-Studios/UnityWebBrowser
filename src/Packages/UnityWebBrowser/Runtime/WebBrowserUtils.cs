using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityWebBrowser
{
    /// <summary>
    ///     Provides Utils to be used by the web browser
    /// </summary>
    public static class WebBrowserUtils
    {
#if UNITY_EDITOR
        /// <summary>
        ///     The name of the package that the web browser is contained in
        /// </summary>
        public const string PackageName = "dev.voltstro.unitywebbrowser";
#endif

        /// <summary>
        ///     Gets the main directory where logs and cache may be stored
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserEngineMainDirectory()
        {
#if UNITY_EDITOR
            return Path.GetFullPath($"{Directory.GetParent(Application.dataPath).FullName}/Library");
#else
			return Application.dataPath;
#endif
        }

        /// <summary>
        ///     Gets the folder that the cef process application lives in
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserEnginePath(string engine)
        {
            //Editor
#if UNITY_EDITOR
            Editor.BrowserEngine browserEngine = Editor.BrowserEngineManager.GetBrowser(engine);

#if UNITY_EDITOR_WIN
            return Path.GetFullPath(browserEngine.BuildFiles.FirstOrDefault(x =>
                x.Key == UnityEditor.BuildTarget.StandaloneWindows ||
                x.Key == UnityEditor.BuildTarget.StandaloneWindows64).Value);
#elif UNITY_EDITOR_LINUX
			return Path.GetFullPath(browserEngine.BuildFiles.FirstOrDefault(x =>
		        x.Key == UnityEditor.BuildTarget.StandaloneLinux64).Value);
#endif

            //Player builds (Standalone)
#elif UNITY_STANDALONE_WIN
		    return Path.GetFullPath($"{Application.dataPath}/Plugins/x86_64/");
#else
		    return Path.GetFullPath($"{Application.dataPath}/Plugins/");
#endif
        }

        /// <summary>
        ///     Get a direct path to the cef browser process application
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserEngineProcessPath(string engine)
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return $"{GetBrowserEnginePath(engine)}{engine}.exe";
#else
		    return $"{GetBrowserEnginePath(engine)}{engine}";
#endif
        }

        /// <summary>
        ///     Gets the local position delta (0 -> 1) from a screen position on a <see cref="RawImage" />
        ///     from a top-left origin point
        ///     <para>
        ///         To calculate the pixel position,
        ///         do <see cref="Vector2.x" /> * [Desired height] and <see cref="Vector2.y" /> * [Desired Width]
        ///     </para>
        /// </summary>
        /// <param name="image"><see cref="RawImage" /> that you want to calculate the local position on</param>
        /// <param name="screenPosition">The screen position</param>
        /// <param name="position">The local delta position</param>
        /// <returns></returns>
        public static bool GetScreenPointToLocalPositionDeltaOnImage(RawImage image, Vector2 screenPosition,
            out Vector2 position)
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

        ///  <summary>
        /// 		Waits until the file exists
        ///  </summary>
        ///  <param name="path"></param>
        ///  <param name="timeOutTIme"></param>
        ///  <param name="onSuccess"></param>
        ///  <param name="onFail"></param>
        ///  <exception cref="TimeoutException"></exception>
        internal static Task WaitForActiveEngineFile(string path, int timeOutTIme, Action onSuccess, Action onFail)
        {
            float timeUntilCancel = Time.time + timeOutTIme;

            //Wait until the file exists
            while (!File.Exists(path))
            {
                //If we hit the timeout time, then fail it
                if (Time.time >= timeUntilCancel)
                {
                    onFail.Invoke();
                    return Task.CompletedTask;
                }

                Thread.Sleep(100);
            }
            
            onSuccess.Invoke();
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sets every single pixel in a <see cref="Texture2D"/> to one <see cref="Color32"/>
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="color"></param>
        internal static void SetAllTextureColorToOne(Texture2D texture, Color32 color)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture), "Texture cannot be null!");
            
            Color32[] colors = new Color32[texture.width * texture.height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            texture.SetPixels32(colors);
            texture.Apply();
        }
    }
}